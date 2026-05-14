namespace Gotcha2.Maui.Constants
{
    public static class RoutesConstants
    {
        // Absolute routes — used with Shell.Current.GoToAsync("//X") to switch tab bars or jump between tabs.
        // These pages are mounted as <ShellContent> in AppShell.xaml, so they skip Routing.RegisterRoute.
        public const string SignIn = "//SignIn";
        public const string SignUp = "//SignUp";
        public const string Info = "//Info";

        public const string Home = "//Home";
        public const string Games = "//Games";
        public const string Settings = "//Settings";

        // Relative routes — pushed onto the current tab's nav stack via Shell.Current.GoToAsync("X").
        // These pages must be registered in MauiProgram.cs with Routing.RegisterRoute.
        public const string NewGame = "NewGame";
        public const string PlayerHome = "PlayerHome";
        public const string ConfirmKill = "ConfirmKill";
        public const string MatchResult = "MatchResult";
    }
}
