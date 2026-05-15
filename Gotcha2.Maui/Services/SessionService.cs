using Gotcha2.Maui.Constants;

namespace Gotcha2.Maui.Services
{
    public class SessionService
    {
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";

        private Guid? _currentUserId;
        private string? _currentAuthToken;

        public Guid? CurrentUserId
        {
            get { return _currentUserId; }
        }

        public string? CurrentAuthToken
        {
            get { return _currentAuthToken; }
        }

        public bool IsSignedIn
        {
            get { return _currentUserId.HasValue && !string.IsNullOrEmpty(_currentAuthToken); }
        }

        /* Populates in-memory session state.
         * Called after a successful sign-in (with the freshly issued token) or by TryRestoreAsync (with values read from SecureStorage).
         * Does NOT persist — caller decides via PersistAsync whether to remember across launches. */
        public void BeginSession(Guid userId, string token)
        {
            _currentUserId = userId;
            _currentAuthToken = token;
        }

        // Writes the token + user id to platform-encoded SecureStorage so the user
        // stays signed in across app launches. Only called when "Remember me" is checked.
        public async Task PersistAsync(Guid userId, string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
            await SecureStorage.SetAsync(UserIdKey, userId.ToString());
        }

        /* Reads a previously-persisted token + user id from SecureStorage and populates the in-memory state.
         * Returns true if a valid session was restored; false if no session was stored or the stored values are invalid.
         * Does NOT navigate — caller (App.OnStart) decides whether to switch to the authenticated TabBar. */
        public async Task<bool> TryRestoreAsync()
        {
            string? token = await SecureStorage.GetAsync(TokenKey);
            string? userIdRaw = await SecureStorage.GetAsync(UserIdKey);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdRaw))
                return false;

            if (!Guid.TryParse(userIdRaw, out Guid userId))
                return false;

            BeginSession(userId, token);

            return true;
        }

        // Switches Shell to the Authenticated TabBar (Home tab).
        public void SwitchToAuthenticatedTabBar()
        {
            Shell.Current.GoToAsync(RoutesConstants.Home);
        }

        // Clears in-memory state + SecureStorage, then returns to the Unauthenticated TabBar.
        public async Task SignOutAsync()
        {
            _currentUserId = null;
            _currentAuthToken = null;

            SecureStorage.Remove(TokenKey);
            SecureStorage.Remove(UserIdKey);

            await Shell.Current.GoToAsync(RoutesConstants.SignIn);
        }
    }
}
