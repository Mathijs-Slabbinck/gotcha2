using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class SignInViewModel : PageBaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly SessionService _sessionService;


        private string email = string.Empty;
        public string Email
        {
            get { return email; }
            set
            {
                SetProperty(ref email, value);
                EmailError = string.Empty;
            }
        }

        private string emailError = string.Empty;
        public string EmailError
        {
            get { return emailError; }
            set { SetProperty(ref emailError, value); }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);
                PasswordError = string.Empty;
            }
        }

        private string passwordError = string.Empty;
        public string PasswordError
        {
            get { return passwordError; }
            set { SetProperty(ref passwordError, value); }
        }

        private bool rememberMe;
        public bool RememberMe
        {
            get { return rememberMe; }
            set { SetProperty(ref rememberMe, value); }
        }


        private bool isPasswordVisible;
        public bool IsPasswordVisible
        {
            get { return isPasswordVisible; }
            set { SetProperty(ref isPasswordVisible, value); }
        }

        public ICommand SignInCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand SignUpCommand { get; }

        public SignInViewModel(IAuthService authService, SessionService sessionService)
        {
            _authService = authService;
            _sessionService = sessionService;

            SignInCommand = new Command(ExecuteSignInCommand);
            TogglePasswordVisibilityCommand = new Command(ExecuteTogglePasswordVisibility);
            SignUpCommand = new Command(ExecuteSignUpCommand);
        }

        private async void ExecuteSignInCommand()
        {
            try
            {
                ResetErrors();

                if (string.IsNullOrWhiteSpace(Email))
                    EmailError = "Email is required.";

                if (string.IsNullOrWhiteSpace(Password))
                    PasswordError = "Password is required.";

                if (!string.IsNullOrEmpty(EmailError) || !string.IsNullOrEmpty(PasswordError))
                    return;

                IsBusy = true;

                ResultModel<AuthResponseDto> result = await _authService.SignInAsync(Email, Password);

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                }
                else if (result.Data is not null)
                {
                    _sessionService.BeginSession(result.Data.UserId, result.Data.Token);

                    if (RememberMe)
                        await _sessionService.PersistAsync(result.Data.UserId, result.Data.Token);

                    await _sessionService.SwitchToAuthenticatedTabBarAsync();
                }
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
            }

            IsBusy = false;
        }

        private void ExecuteTogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private async void ExecuteSignUpCommand()
        {
            try
            {
                await Shell.Current.GoToAsync(RoutesConstants.SignUp);
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
            }
        }

        private void ResetErrors()
        {
            // Clear field-specific errors from previous attempts.
            EmailError = string.Empty;
            PasswordError = string.Empty;

            // Clear API errors from previous attempts.
            Errors = new List<string>();
        }
    }
}
