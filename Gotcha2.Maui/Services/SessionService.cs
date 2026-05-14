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

        public void SetUser(Guid userId)
        {
            _currentUserId = userId;
        }

        public void SetAuthToken(string token)
        {
            _currentAuthToken = token;
        }

        // Writes the token + user id to platform-encrypted SecureStorage so the user
        // stays signed in across app launches. Only called when "Remember me" is checked.
        public async Task PersistAsync(Guid userId, string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
            await SecureStorage.SetAsync(UserIdKey, userId.ToString());
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
