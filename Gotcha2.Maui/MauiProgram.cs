using CommunityToolkit.Maui;
using FluentValidation;
using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Models.Forms;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.Services.Api;
using Gotcha2.Maui.Services.Http;
using Gotcha2.Maui.Validators;
using Gotcha2.Maui.ViewModels;
using Microsoft.Extensions.Logging;

using SignIn = Gotcha2.Maui.Pages.Unauthenticated.SignIn;
using SignUp = Gotcha2.Maui.Pages.Unauthenticated.SignUp;
using Info = Gotcha2.Maui.Pages.Unauthenticated.Info;
using Home = Gotcha2.Maui.Pages.Authenticated.Home;
using Games = Gotcha2.Maui.Pages.Authenticated.Games;
using Settings = Gotcha2.Maui.Pages.Authenticated.Settings;
using NewGame = Gotcha2.Maui.Pages.Authenticated.NewGame;
using PlayerHome = Gotcha2.Maui.Pages.Authenticated.PlayerHome;
using ConfirmKill = Gotcha2.Maui.Pages.Authenticated.ConfirmKill;

namespace Gotcha2.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // === Routing ===
            // TabBar-mounted pages (SignIn/SignUp/Info/Home/Games/Settings) skip Routing.RegisterRoute —
            // they're declared as <ShellContent> in AppShell.xaml and Shell resolves them by Route attribute.
            // Relative routes pushed onto the current tab's nav stack must be registered here.
            // Phase 6 — uncomment as they land:
            Routing.RegisterRoute(RoutesConstants.NewGame, typeof(NewGame));
            Routing.RegisterRoute(RoutesConstants.PlayerHome, typeof(PlayerHome));
            Routing.RegisterRoute(RoutesConstants.ConfirmKill, typeof(ConfirmKill));

            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");    // used by: default Button style (template)
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");  // used by: (none yet)
                    fonts.AddFont("Bungee-Regular.ttf", "Bungee");               // used by: SignIn (title)
                    fonts.AddFont("RobotoSlab-Regular.ttf", "RobotoSlab");       // used by: SignIn (card header, CTA card header)
                    fonts.AddFont("RobotoSlab-Bold.ttf", "RobotoSlabBold");      // used by: (none yet)
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // === Session ===
            // Singleton — holds CurrentUserId + CurrentAuthToken across the app lifetime.
            builder.Services.AddSingleton<SessionService>();

            // === HTTP handler chain ===
            // Transient because HttpClientFactory rotates the handler pool periodically (default 2 min).
            // Order in AddHttpMessageHandler below: first-added is outermost.
            builder.Services.AddTransient<AuthHeaderHandler>();
            builder.Services.AddTransient<UnauthorizedHandler>();

            // === Named HttpClient ===
            // Every Api*Service resolves this via IHttpClientFactory.CreateClient("GotchaApi").
            // AuthHeaderHandler runs OUTER (stamps token on outbound). UnauthorizedHandler runs INNER (sees 401 on inbound).
            builder.Services.AddHttpClient("GotchaApi", client =>
            {
                client.BaseAddress = new Uri(ApiHostConstants.BaseUrl);
                // Fail fast on hangs (default is 100s). Keeps the UI from locking up if the API/tunnel is unreachable —
                // TaskCanceledException bubbles up to each Api*Service's catch and the user sees an error within 15s.
                client.Timeout = TimeSpan.FromSeconds(15);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<UnauthorizedHandler>();

            // === Services (Phase 7) ===
            builder.Services.AddTransient<IAuthService, ApiAuthService>();
            builder.Services.AddTransient<IGameService, ApiGameService>();
            builder.Services.AddTransient<IPlayerService, ApiPlayerService>();
            builder.Services.AddTransient<IUserService, ApiUserService>();

            // === AppShell ===
            // Singleton so DI can inject SessionService into it (Phase 6 wires the ctor).
            builder.Services.AddSingleton<AppShell>();

            // === ViewModels (Phase 6) ===
            // Transient — fresh state per navigation. Uncomment as VMs land:
            builder.Services.AddTransient<SignInViewModel>();
            builder.Services.AddTransient<SignUpViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<GamesViewModel>();
            builder.Services.AddTransient<NewGameViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<PlayerHomeViewModel>();
            builder.Services.AddTransient<ConfirmKillViewModel>();

            // === Validators (Phase 9) ===
            // Transient — stateless rule definitions; one instance per VM injection.
            builder.Services.AddTransient<IValidator<SignUpData>, SignUpValidator>();
            builder.Services.AddTransient<IValidator<SettingsData>, SettingsValidator>();
            builder.Services.AddTransient<IValidator<ChangeMyPasswordData>, ChangeMyPasswordValidator>();

            // === Pages ===
            // Transient — DI resolves and injects the VM (once VMs are wired in Phase 6).
            // Currently page stubs without BindingContext; that's fine — they just render placeholder content.
            builder.Services.AddTransient<SignIn>();
            builder.Services.AddTransient<SignUp>();
            builder.Services.AddTransient<Info>();
            builder.Services.AddTransient<Home>();
            builder.Services.AddTransient<Games>();
            builder.Services.AddTransient<Settings>();
            // Phase 6 — relative-route pages:
            builder.Services.AddTransient<NewGame>();
            builder.Services.AddTransient<PlayerHome>();
            builder.Services.AddTransient<ConfirmKill>();

            return builder.Build();
        }
    }
}
