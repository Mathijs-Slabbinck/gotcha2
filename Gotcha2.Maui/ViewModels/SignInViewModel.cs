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
            set { SetProperty(ref email, value); }
        }

        private string password = string.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
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
                Errors = new List<string>();

                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    Errors = new List<string> { "Please fill in all fields." };
                    return;
                }

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
    }
}
