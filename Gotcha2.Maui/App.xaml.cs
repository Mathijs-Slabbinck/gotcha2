using Gotcha2.Maui.Services;

namespace Gotcha2.Maui
{
    public partial class App : Application
    {
        private readonly SessionService _sessionService;
        private readonly AppShell _appShell;

        public App(SessionService sessionService, AppShell appShell)
        {
            InitializeComponent();
            _sessionService = sessionService;
            _appShell = appShell;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(_appShell);
        }

        protected override async void OnStart()
        {
            /* Runs the parent Application's OnStart
             * fires the Start event for any framework / plugin subscribers.
             * Skipping it silently disables those hooks. */
            base.OnStart();

            try
            {
                // TryResumeAsync self-marshals the nav call; we just await.
                await _sessionService.TryResumeAsync();
            }
            catch (Exception ex)
            {
                // Normally I would use a logging framework,
                // but a simple Debug.WriteLine is sufficient for this example and easier to explain
                System.Diagnostics.Debug.WriteLine($"App.OnStart failed: {ex.Message}");
            }
        }
    }
}
