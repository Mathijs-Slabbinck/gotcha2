using FluentValidation;
using FluentValidation.Results;
using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Enums;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Forms;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class SignUpViewModel : PageBaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly SessionService _sessionService;
        private readonly IValidator<SignUpData> _validator;


        private string firstName = string.Empty;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                SetProperty(ref firstName, value);
                FirstNameError = string.Empty;
            }
        }

        private string firstNameError = string.Empty;
        public string FirstNameError
        {
            get { return firstNameError; }
            set { SetProperty(ref firstNameError, value); }
        }

        private string lastName = string.Empty;
        public string LastName
        {
            get { return lastName; }
            set
            {
                SetProperty(ref lastName, value);
                LastNameError = string.Empty;
            }
        }

        private string lastNameError = string.Empty;
        public string LastNameError
        {
            get { return lastNameError; }
            set { SetProperty(ref lastNameError, value); }
        }

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

        private string confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                SetProperty(ref confirmPassword, value);
                ConfirmPasswordError = string.Empty;
            }
        }

        private string confirmPasswordError = string.Empty;
        public string ConfirmPasswordError
        {
            get { return confirmPasswordError; }
            set { SetProperty(ref confirmPasswordError, value); }
        }

        private DateTime birthDate = DateTime.Today;
        public DateTime BirthDate
        {
            get { return birthDate; }
            set
            {
                SetProperty(ref birthDate, value);
                BirthDateError = string.Empty;
            }
        }

        private string birthDateError = string.Empty;
        public string BirthDateError
        {
            get { return birthDateError; }
            set { SetProperty(ref birthDateError, value); }
        }

        private Genders? selectedGender;
        public Genders? SelectedGender
        {
            get { return selectedGender; }
            set
            {
                SetProperty(ref selectedGender, value);
                GenderError = string.Empty;
            }
        }

        private string genderError = string.Empty;
        public string GenderError
        {
            get { return genderError; }
            set { SetProperty(ref genderError, value); }
        }

        public List<Genders> GenderOptions { get; }

        private bool rememberMe = true;
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

        private bool isConfirmPasswordVisible;
        public bool IsConfirmPasswordVisible
        {
            get { return isConfirmPasswordVisible; }
            set { SetProperty(ref isConfirmPasswordVisible, value); }
        }

        public ICommand SignUpCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand ChoosePhotoCommand { get; }
        public ICommand SignInCommand { get; }

        public SignUpViewModel(IAuthService authService, SessionService sessionService, IValidator<SignUpData> validator)
        {
            _authService = authService;
            _sessionService = sessionService;
            _validator = validator;

            GenderOptions = new List<Genders> { Genders.Male, Genders.Female, Genders.Other };

            SignUpCommand = new Command(ExecuteSignUpCommand);
            TogglePasswordVisibilityCommand = new Command(ExecuteTogglePasswordVisibility);
            ToggleConfirmPasswordVisibilityCommand = new Command(ExecuteToggleConfirmPasswordVisibility);
            TakePhotoCommand = new Command(ExecuteTakePhotoCommand);
            ChoosePhotoCommand = new Command(ExecuteChoosePhotoCommand);
            SignInCommand = new Command(ExecuteSignInCommand);
        }

        private async void ExecuteSignUpCommand()
        {
            try
            {
                ResetErrors();

                SignUpData form = new SignUpData
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword,
                    BirthDate = BirthDate,
                    Gender = SelectedGender
                };

                ValidationResult validationResult = _validator.Validate(form);

                if (!validationResult.IsValid)
                {
                    DispatchValidationFailures(validationResult);
                    return;
                }

                IsBusy = true;

                RegisterRequestDto request = new RegisterRequestDto
                {
                    Email = Email,
                    Password = Password,
                    FirstName = FirstName,
                    LastName = LastName,
                    BirthDate = BirthDate,
                    Gender = SelectedGender!.Value
                };

                ResultModel<AuthResponseDto> result = await _authService.RegisterAsync(request);

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

        private void DispatchValidationFailures(ValidationResult validationResult)
        {
            foreach (ValidationFailure failure in validationResult.Errors)
            {
                if (failure.PropertyName == nameof(SignUpData.FirstName))
                {
                    FirstNameError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SignUpData.LastName))
                {
                    LastNameError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SignUpData.Email))
                {
                    EmailError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SignUpData.Password))
                {
                    PasswordError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SignUpData.ConfirmPassword))
                {
                    ConfirmPasswordError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SignUpData.BirthDate))
                {
                    BirthDateError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SignUpData.Gender))
                {
                    GenderError = failure.ErrorMessage;
                }
                else
                {
                    // Drift-catcher: if a new RuleFor lands in the validator without a matching VM slot, fail loudly on first submit.
                    throw new NotImplementedException(
                        $"The property {failure.PropertyName} is not handled in the viewmodel");
                }
            }
        }

        private void ExecuteTogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void ExecuteToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }

        private void ExecuteTakePhotoCommand()
        {
            // Phase 8 — wires MediaPicker.Default.CapturePhotoAsync + upload to PUT /api/users/me/profile-image.
        }

        private void ExecuteChoosePhotoCommand()
        {
            // Phase 8 — wires MediaPicker.Default.PickPhotoAsync + upload to PUT /api/users/me/profile-image.
        }

        private async void ExecuteSignInCommand()
        {
            try
            {
                await Shell.Current.GoToAsync(RoutesConstants.SignIn);
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
            }
        }

        private void ResetErrors()
        {
            FirstNameError = string.Empty;
            LastNameError = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
            BirthDateError = string.Empty;
            GenderError = string.Empty;

            Errors = new List<string>();
        }
    }
}
