using Gotcha2.Maui.Constants;

namespace Gotcha2.Maui.Services
{
    public class SessionService
    {
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";

        private Guid? currentUserId;
        private string? currentAuthToken;

        public Guid? CurrentUserId
        {
            get { return currentUserId; }
        }

        public string? CurrentAuthToken
        {
            get { return currentAuthToken; }
        }

        public bool IsSignedIn
        {
            get { return currentUserId.HasValue && !string.IsNullOrEmpty(currentAuthToken); }
        }

        /* Populates in-memory session state.
         * Called after a successful sign-in (with the freshly issued token) or by TryRestoreAsync (with values read from SecureStorage).
         * Does NOT persist — caller decides via PersistAsync whether to remember across launches. */
        public void BeginSession(Guid userId, string token)
        {
            currentUserId = userId;
            currentAuthToken = token;
        }

        // Writes the token + user id to platform-encoded SecureStorage so the user
        // stays signed in across app launches. Only called when "Remember me" is checked.
        public async Task PersistAsync(Guid userId, string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
            await SecureStorage.SetAsync(UserIdKey, userId.ToString());
        }

        /* Reads a previously-persisted token + user id from SecureStorage, populates in-memory state,
         * and switches to the Authenticated TabBar. Returns true if a valid session was resumed;
         * false if nothing was stored or the stored values were invalid.
         * Self-marshals the nav call to the UI thread — callers can just await. */
        public async Task<bool> TryResumeAsync()
        {
            string? token = await SecureStorage.GetAsync(TokenKey);
            string? userIdRaw = await SecureStorage.GetAsync(UserIdKey);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdRaw))
                return false;

            if (!Guid.TryParse(userIdRaw, out Guid userId))
                return false;

            BeginSession(userId, token);

            await MainThread.InvokeOnMainThreadAsync(SwitchToAuthenticatedTabBarAsync);

            return true;
        }

        // Switches Shell to the Authenticated TabBar (Home tab). Must run on the UI thread.
        public async Task SwitchToAuthenticatedTabBarAsync()
        {
            await Shell.Current.GoToAsync(RoutesConstants.Home);
        }

        /* Clears in-memory state + SecureStorage, then returns to the Unauthenticated TabBar.
         * Self-marshals the nav call to the UI thread — callers can just await. */
        public async Task SignOutAsync()
        {
            currentUserId = null;
            currentAuthToken = null;

            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(UserIdKey);

            await MainThread.InvokeOnMainThreadAsync(async () => await Shell.Current.GoToAsync(RoutesConstants.SignIn));
        }
    }
}
