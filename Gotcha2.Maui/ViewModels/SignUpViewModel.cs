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
using System.ComponentModel;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class SignUpViewModel : PageBaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly SessionService _sessionService;
        private readonly IValidator<SignUpData> _validator;
        private readonly IUserService _userService;

        // Photo upload constants — mirror the API's PUT /api/users/me/profile-image gate.
        private const long MaxPhotoBytes = 5 * 1024 * 1024;
        private const string JpegMimeType = "image/jpeg";
        private const string PngMimeType = "image/png";
        private const string DefaultPhotoFileName = "photo.jpg";


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

        // Picked photo state — bytes + source for preview, contentType + fileName for upload.
        // ContentType + FileName stay as plain fields because no XAML binds to them.
        private string? pickedPhotoContentType;
        private string? pickedPhotoFileName;

        private byte[]? pickedPhotoBytes;
        public byte[]? PickedPhotoBytes
        {
            get { return pickedPhotoBytes; }
            set
            {
                SetProperty(ref pickedPhotoBytes, value);
                OnPropertyChanged(nameof(HasPickedPhoto));
            }
        }

        private ImageSource? pickedPhotoSource;
        public ImageSource? PickedPhotoSource
        {
            get { return pickedPhotoSource; }
            set { SetProperty(ref pickedPhotoSource, value); }
        }

        public bool HasPickedPhoto
        {
            get { return pickedPhotoBytes != null; }
        }

        private string photoError = string.Empty;
        public string PhotoError
        {
            get { return photoError; }
            set
            {
                SetProperty(ref photoError, value);
                OnPropertyChanged(nameof(CanSubmit));
            }
        }

        // Toggled to true only when register + BeginSession both succeeded but the photo upload failed.
        // Hides the Submit row and reveals the Retry / Skip row.
        private bool isAwaitingUploadRetry;
        public bool IsAwaitingUploadRetry
        {
            get { return isAwaitingUploadRetry; }
            set { SetProperty(ref isAwaitingUploadRetry, value); }
        }

        // Combined gate for the Submit button — disabled while busy OR while a pre-flight photo error is showing.
        // CanSubmit refreshes via the OnPropertyChanged override (on IsBusy) and via PhotoError's setter.
        public bool CanSubmit
        {
            get { return !IsBusy && string.IsNullOrEmpty(photoError); }
        }

        public ICommand SignUpCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand ChoosePhotoCommand { get; }
        public ICommand RemovePhotoCommand { get; }
        public ICommand RetryUploadCommand { get; }
        public ICommand SkipPhotoAndContinueCommand { get; }
        public ICommand SignInCommand { get; }

        public SignUpViewModel(IAuthService authService, SessionService sessionService, IValidator<SignUpData> validator, IUserService userService)
        {
            _authService = authService;
            _sessionService = sessionService;
            _validator = validator;
            _userService = userService;

            GenderOptions = new List<Genders> { Genders.Male, Genders.Female, Genders.Other };

            SignUpCommand = new Command(ExecuteSignUpCommand);
            TogglePasswordVisibilityCommand = new Command(ExecuteTogglePasswordVisibility);
            ToggleConfirmPasswordVisibilityCommand = new Command(ExecuteToggleConfirmPasswordVisibility);
            TakePhotoCommand = new Command(ExecuteTakePhotoCommand);
            ChoosePhotoCommand = new Command(ExecuteChoosePhotoCommand);
            RemovePhotoCommand = new Command(ExecuteRemovePhotoCommand);
            RetryUploadCommand = new Command(ExecuteRetryUploadCommand);
            SkipPhotoAndContinueCommand = new Command(ExecuteSkipPhotoAndContinueCommand);
            SignInCommand = new Command(ExecuteSignInCommand);
        }

        // CanSubmit depends on IsBusy (base) and PhotoError (local).
        // PhotoError's setter raises CanSubmit directly; IsBusy is raised from the base, so we re-fire here.
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(CanSubmit));
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

                    // Bearer token is now set on SessionService → AuthHeaderHandler will stamp it on the upload call.
                    // Pre-flight passed back when the photo was picked; the only residual failure here is a network glitch
                    // mid-flight. On failure we enter the retry phase (UI hides Submit, shows Retry + Skip).
                    if (HasPickedPhoto)
                    {
                        bool uploadedOk = await UploadPhotoAsync();

                        if (!uploadedOk)
                        {
                            IsAwaitingUploadRetry = true;
                            IsBusy = false;
                            return;
                        }
                    }

                    await _sessionService.SwitchToAuthenticatedTabBarAsync();
                }

                IsBusy = false;
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
                IsBusy = false;
            }
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

        private async void ExecuteTakePhotoCommand()
        {
            try
            {
                PhotoError = string.Empty;

                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    PhotoError = "Camera is not supported on this device.";
                    return;
                }

                PermissionStatus status = await Permissions.RequestAsync<Permissions.Camera>();

                if (status != PermissionStatus.Granted)
                {
                    PhotoError = "Camera permission denied.";
                    return;
                }

                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

                // Null = user cancelled the picker. Bail silently (no error).
                if (photo == null)
                    return;

                await ProcessPickedPhotoAsync(photo);
            }
            catch
            {
                PhotoError = "Could not capture photo. Please try again.";
            }
        }

        private async void ExecuteChoosePhotoCommand()
        {
            try
            {
                PhotoError = string.Empty;

                // PickPhotoAsync is deprecated in current MAUI — PickPhotosAsync is the supported single + multi entrypoint.
                // We only care about the first one (UX is single-photo).
                IEnumerable<FileResult> photos = await MediaPicker.Default.PickPhotosAsync();
                FileResult? photo = photos.FirstOrDefault();

                // Null = user cancelled the picker. Bail silently (no error).
                if (photo == null)
                    return;

                await ProcessPickedPhotoAsync(photo);
            }
            catch
            {
                PhotoError = "Could not load photo. Please try again.";
            }
        }

        private void ExecuteRemovePhotoCommand()
        {
            ClearPickedPhoto();
        }

        private async void ExecuteRetryUploadCommand()
        {
            try
            {
                IsBusy = true;

                bool uploadedOk = await UploadPhotoAsync();

                if (uploadedOk)
                {
                    IsAwaitingUploadRetry = false;
                    await _sessionService.SwitchToAuthenticatedTabBarAsync();
                }
                // else: Errors was refreshed by UploadPhotoAsync; stay in retry phase for another try or Skip.

                IsBusy = false;
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
                IsBusy = false;
            }
        }

        private async void ExecuteSkipPhotoAndContinueCommand()
        {
            try
            {
                ClearPickedPhoto();
                IsAwaitingUploadRetry = false;
                Errors = new List<string>();
                await _sessionService.SwitchToAuthenticatedTabBarAsync();
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
            }
        }

        private async Task ProcessPickedPhotoAsync(FileResult photo)
        {
            // Read the FileResult stream once into memory. 5 MB cap means in-memory is fine, and it lets the
            // preview ImageSource and the upload share the same byte[] instead of re-opening the stream
            // (which on Android can be a content:// URI that's not always re-openable).
            using Stream stream = await photo.OpenReadAsync();
            using MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();

            string contentType;

            if (!string.IsNullOrEmpty(photo.ContentType))
                contentType = photo.ContentType;
            else
                contentType = JpegMimeType;

            string fileName;

            if (!string.IsNullOrEmpty(photo.FileName))
                fileName = photo.FileName;
            else
                fileName = DefaultPhotoFileName;

            BaseResultModel preflight = RunPhotoPreflight(bytes, contentType);

            if (!preflight.IsSuccess)
            {
                // Leave any previously-picked valid photo in place — user can Remove it or pick another.
                PhotoError = preflight.Errors.FirstOrDefault() ?? "Invalid photo.";
                return;
            }

            pickedPhotoContentType = contentType;
            pickedPhotoFileName = fileName;
            PickedPhotoSource = ImageSource.FromStream(() => new MemoryStream(bytes));
            PickedPhotoBytes = bytes;
        }

        private static BaseResultModel RunPhotoPreflight(byte[] bytes, string contentType)
        {
            BaseResultModel result = new BaseResultModel();

            if (bytes.Length > MaxPhotoBytes)
            {
                result.Errors.Add("Photo too large (max 5 MB).");
                return result;
            }

            bool isJpeg = string.Equals(contentType, JpegMimeType, StringComparison.OrdinalIgnoreCase);
            bool isPng = string.Equals(contentType, PngMimeType, StringComparison.OrdinalIgnoreCase);

            if (!isJpeg && !isPng)
                result.Errors.Add("Only JPEG or PNG photos are accepted.");

            return result;
        }

        private async Task<bool> UploadPhotoAsync()
        {
            // Caller has already checked HasPickedPhoto, so the three photo fields are non-null here.
            BaseResultModel result = await _userService.UpdateProfileImageAsync(
                pickedPhotoBytes!,
                pickedPhotoContentType!,
                pickedPhotoFileName!);

            if (!result.IsSuccess)
            {
                Errors = result.Errors;
                return false;
            }

            return true;
        }

        private void ClearPickedPhoto()
        {
            PickedPhotoBytes = null;
            PickedPhotoSource = null;
            pickedPhotoContentType = null;
            pickedPhotoFileName = null;
            PhotoError = string.Empty;
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

            // Photo errors are cleared only on the pick path (or on Remove). ResetErrors fires on Submit;
            // a lingering PhotoError there means the pre-flight Submit-gate is still active and we want it to stay so.
            // IsAwaitingUploadRetry is cleared on Skip / successful Retry — also not here.

            Errors = new List<string>();
        }
    }
}
