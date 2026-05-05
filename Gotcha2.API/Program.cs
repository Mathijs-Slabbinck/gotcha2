using System.Text;
using Gotcha2.API.Constants.Contracts;
using Gotcha2.Core.Data;
using Gotcha2.Core.Data.Seeder;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// === Controllers + unified 400 shape ===
// Collapse the default ValidationProblemDetails body into a flat string[] so it matches our
// other 400 shapes (BadRequest(string[]) returns in AuthController). One shape for the consumer.
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        string[] errors = context.ModelState
            .SelectMany(entry => entry.Value!.Errors.Select(error => error.ErrorMessage))
            .ToArray();
        return new BadRequestObjectResult(errors);
    };
});

builder.Services.AddEndpointsApiExplorer();

// === Swagger + JWT bearer button ===
builder.Services.AddSwaggerGen(options =>
{
    OpenApiSecurityScheme jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste your JWT here. No need to type 'Bearer ' — Swagger adds it.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtScheme);

    OpenApiSecurityRequirement requirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    };
    options.AddSecurityRequirement(requirement);
});

// === DbContext ===
string? connectionString = builder.Configuration.GetConnectionString("Gotcha2DBContext");
if (connectionString == null)
{
    throw new InvalidOperationException("Connection string 'Gotcha2DBContext' not found.");
}

builder.Services.AddDbContext<Gotcha2DBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// === Identity ===
builder.Services
    .AddIdentity<GotchaUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.User.RequireUniqueEmail = true;

        // Lockout — anti-bruteforce on /api/auth/login.
        // 5 wrong passwords → 15-minute auto-unlock. Per-user, not per-IP.
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<Gotcha2DBContext>()
    .AddDefaultTokenProviders();

// === JWT authentication ===
string jwtKey = builder.Configuration[JwtConfigKeys.Key]
    ?? throw new InvalidOperationException($"{JwtConfigKeys.Key} not configured.");
string jwtIssuer = builder.Configuration[JwtConfigKeys.Issuer]
    ?? throw new InvalidOperationException($"{JwtConfigKeys.Issuer} not configured.");
string jwtAudience = builder.Configuration[JwtConfigKeys.Audience]
    ?? throw new InvalidOperationException($"{JwtConfigKeys.Audience} not configured.");

SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            // Small drift tolerance — DEVIATES from reference (TimeSpan.Zero) per Gotcha2 decision.
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

// === Repositories ===
builder.Services.AddScoped<IPlayerRepoService, PlayerRepoService>();
builder.Services.AddScoped<IGameRepoService, GameRepoService>();
builder.Services.AddScoped<IKillRepoService, KillRepoService>();
builder.Services.AddScoped<ITargetAssignmentRepoService, TargetAssignmentRepoService>();

// === CORS ===
string[] allowedOrigins = builder.Configuration
    .GetSection(CorsConfigKeys.AllowedOrigins)
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
        // No AllowCredentials — we use bearer headers, not cookies.
    });
});

WebApplication app = builder.Build();

// === Role seed (must run before any registration) ===
using (IServiceScope scope = app.Services.CreateScope())
{
    RoleManager<IdentityRole<Guid>> roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await Seeder.SeedRolesAsync(roleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS must run BEFORE auth — preflight (OPTIONS) requests don't carry the Authorization header,
// so the auth middleware would reject them and the browser would never see the CORS response.
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
