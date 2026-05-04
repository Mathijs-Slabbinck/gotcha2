using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gotcha2.API.Constants.Contracts;
using Gotcha2.API.Dtos.Authentication.Request;
using Gotcha2.API.Dtos.Authentication.Response;
using Gotcha2.Core.Constants;
using Gotcha2.Core.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gotcha2.API.Controllers
{
    // POST /api/auth/register
    // POST /api/auth/login
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<GotchaUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<GotchaUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        #region === REGISTER ===

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            // [ApiController] already runs DataAnnotations on `dto` and returns 400 if any fail.
            GotchaUser? existingByEmail = await _userManager.FindByEmailAsync(dto.Email);

            if (existingByEmail is not null)
            {
                return BadRequest(new[] { "A user with that email already exists." });
            }

            // Identity requires UserName to be populated. Login is email-only, so we mirror Email.
            GotchaUser user = new GotchaUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate!.Value,
                Gender = dto.Gender!.Value
            };

            IdentityResult createResult = await _userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors.Select(e => e.Description));
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(user, Roles.User);

            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors.Select(e => e.Description));
            }

            AuthResponseDto response = await GenerateTokenAsync(user);

            // 201 (not CreatedAtAction): a JWT isn't an addressable resource.
            return StatusCode(StatusCodes.Status201Created, response);
        }

        #endregion

        #region === LOGIN ===

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            GotchaUser? user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                // Generic message — don't leak which emails exist.
                return Unauthorized(new[] { "Invalid email or password." });
            }

            // Short-circuit BEFORE password check — otherwise a correct guess during lockout would still log in.
            if (await _userManager.IsLockedOutAsync(user))
            {
                return Unauthorized(new[] { "Account temporarily locked. Try again later." });
            }

            bool passwordOk = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordOk)
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new[] { "Invalid email or password." });
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            AuthResponseDto response = await GenerateTokenAsync(user);
            return Ok(response);
        }

        #endregion

        #region === TOKEN GENERATION ===

        private async Task<AuthResponseDto> GenerateTokenAsync(GotchaUser user)
        {
            string jwtKey = _configuration[JwtConfigKeys.Key]!;
            string jwtIssuer = _configuration[JwtConfigKeys.Issuer]!;
            string jwtAudience = _configuration[JwtConfigKeys.Audience]!;

            int expiresInMinutes;
            string? configValue = _configuration[JwtConfigKeys.ExpiresInMinutes];
            bool success = int.TryParse(configValue, out int parsed);

            if (success)
            {
                expiresInMinutes = parsed;
            }
            else
            {
                expiresInMinutes = 60;
            }

            // Guid-keyed Identity — must .ToString() the user id for both Sub and NameIdentifier
            // so ClaimsPrincipalExtensions.GetUserId() can Guid.Parse it back out.
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!)
            };

            IList<string> roles = await _userManager.GetRolesAsync(user);

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            DateTime expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDto
            {
                Token = tokenString,
                ExpiresAtUtc = expiresAt
            };
        }

        #endregion
    }
}
