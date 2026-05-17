using FluentValidation;
using FluentValidation.Results;
using Gotcha2.Maui.Enums;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Forms;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class SettingsViewModel : PageBaseViewModel
    {
        private readonly IUserService _userService;
        private readonly SessionService _sessionService;
        private readonly IValidator<SettingsData> _settingsValidator;
        private readonly IValidator<ChangeMyPasswordData> _changeMyPasswordValidator;

        // Photo upload constants — mirror the API's PUT /api/users/me/profile-image gate.
        // Intentionally duplicated with SignUpViewModel; per convention §10 ("avoid over-abstracting"),
        // the two pages don't share a base. Extract to Constants/ if a third consumer lands.
        private const long MaxPhotoBytes = 5 * 1024 * 1024;
        private const string JpegMimeType = "image/jpeg";
        private const string PngMimeType = "image/png";
        private const string DefaultPhotoFileName = "photo.jpg";

        // ============================================================
        // === DETAILS card (Email / FirstName / LastName / Gender) ===
        // ============================================================

        private string email = string.Empty;
        public string Email
        {
            get { return email; }
            set
            {
                SetProperty(ref email, value);
                EmailError = string.Empty;
                DetailsSaved = false;
            }
        }

        private string emailError = string.Empty;
        public string EmailError
        {
            get { return emailError; }
            set { SetProperty(ref emailError, value); }
        }

        private string firstName = string.Empty;
        public string FirstName
        {
            get { return firstName; }
            set
            {
                SetProperty(ref firstName, value);
                FirstNameError = string.Empty;
                DetailsSaved = false;
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
                DetailsSaved = false;
            }
        }

        private string lastNameError = string.Empty;
        public string LastNameError
        {
            get { return lastNameError; }
            set { SetProperty(ref lastNameError, value); }
        }

        private Genders? selectedGender;
        public Genders? SelectedGender
        {
            get { return selectedGender; }
            set
            {
                SetProperty(ref selectedGender, value);
                GenderError = string.Empty;
                DetailsSaved = false;
            }
        }

        private string genderError = string.Empty;
        public string GenderError
        {
            get { return genderError; }
            set { SetProperty(ref genderError, value); }
        }

        public List<Genders> GenderOptions { get; }

        // Read-only display of the user's birthday — populated by LoadDataAsync. BirthDate is locked after
        // signup (see UserUpdateRequestDto on the API), so the page shows it but doesn't bind to an editable control.
        private string birthDateDisplay = string.Empty;
        public string BirthDateDisplay
        {
            get { return birthDateDisplay; }
            set { SetProperty(ref birthDateDisplay, value); }
        }

        // Per-card API error surface — keeps Details failures out of the Photo and Password cards.
        // Joined with "\n" at assignment-time so XAML can bind a single Label.
        private string detailsApiError = string.Empty;
        public string DetailsApiError
        {
            get { return detailsApiError; }
            set { SetProperty(ref detailsApiError, value); }
        }

        // Transient "Saved." confirmation — flipped true on success, cleared on next field edit or next submit.
        private bool detailsSaved;
        public bool DetailsSaved
        {
            get { return detailsSaved; }
            set { SetProperty(ref detailsSaved, value); }
        }

        // Submit gate: disabled while busy. Field-level validation runs at submit-time, not here.
        public bool CanSaveDetails
        {
            get { return !IsBusy; }
        }

        // ============================================================
        // === PASSWORD card (Current / New / Confirm) ===
        // ============================================================

        private string currentPassword = string.Empty;
        public string CurrentPassword
        {
            get { return currentPassword; }
            set
            {
                SetProperty(ref currentPassword, value);
                CurrentPasswordError = string.Empty;
                PasswordSuccess = false;
            }
        }

        private string currentPasswordError = string.Empty;
        public string CurrentPasswordError
        {
            get { return currentPasswordError; }
            set { SetProperty(ref currentPasswordError, value); }
        }

        private string newPassword = string.Empty;
        public string NewPassword
        {
            get { return newPassword; }
            set
            {
                SetProperty(ref newPassword, value);
                NewPasswordError = string.Empty;
                PasswordSuccess = false;
            }
        }

        private string newPasswordError = string.Empty;
        public string NewPasswordError
        {
            get { return newPasswordError; }
            set { SetProperty(ref newPasswordError, value); }
        }

        private string confirmNewPassword = string.Empty;
        public string ConfirmNewPassword
        {
            get { return confirmNewPassword; }
            set
            {
                SetProperty(ref confirmNewPassword, value);
                ConfirmNewPasswordError = string.Empty;
                PasswordSuccess = false;
            }
        }

        private string confirmNewPasswordError = string.Empty;
        public string ConfirmNewPasswordError
        {
            get { return confirmNewPasswordError; }
            set { SetProperty(ref confirmNewPasswordError, value); }
        }

        private bool isCurrentPasswordVisible;
        public bool IsCurrentPasswordVisible
        {
            get { return isCurrentPasswordVisible; }
            set { SetProperty(ref isCurrentPasswordVisible, value); }
        }

        private bool isNewPasswordVisible;
        public bool IsNewPasswordVisible
        {
            get { return isNewPasswordVisible; }
            set { SetProperty(ref isNewPasswordVisible, value); }
        }

        private bool isConfirmNewPasswordVisible;
        public bool IsConfirmNewPasswordVisible
        {
            get { return isConfirmNewPasswordVisible; }
            set { SetProperty(ref isConfirmNewPasswordVisible, value); }
        }

        private string passwordApiError = string.Empty;
        public string PasswordApiError
        {
            get { return passwordApiError; }
            set { SetProperty(ref passwordApiError, value); }
        }

        // Transient "Password updated." confirmation — flipped true on success, cleared on next field edit or submit.
        private bool passwordSuccess;
        public bool PasswordSuccess
        {
            get { return passwordSuccess; }
            set { SetProperty(ref passwordSuccess, value); }
        }

        public bool CanChangePassword
        {
            get { return !IsBusy; }
        }

        // ============================================================
        // === PHOTO card ===
        // ============================================================

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
                OnPropertyChanged(nameof(CanSubmitPhoto));
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
                OnPropertyChanged(nameof(CanSubmitPhoto));
            }
        }

        public bool CanSubmitPhoto
        {
            get { return !IsBusy && HasPickedPhoto && string.IsNullOrEmpty(photoError); }
        }

        // ============================================================
        // === Commands ===
        // ============================================================

        public ICommand SaveDetailsCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand SignOutCommand { get; }
        public ICommand ToggleCurrentPasswordVisibilityCommand { get; }
        public ICommand ToggleNewPasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmNewPasswordVisibilityCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand ChoosePhotoCommand { get; }
        public ICommand RemovePhotoCommand { get; }
        public ICommand SubmitPhotoCommand { get; }

        public SettingsViewModel(
            IUserService userService,
            SessionService sessionService,
            IValidator<SettingsData> settingsValidator,
            IValidator<ChangeMyPasswordData> changeMyPasswordValidator)
        {
            _userService = userService;
            _sessionService = sessionService;
            _settingsValidator = settingsValidator;
            _changeMyPasswordValidator = changeMyPasswordValidator;

            GenderOptions = new List<Genders> { Genders.Male, Genders.Female, Genders.Other };

            SaveDetailsCommand = new Command(ExecuteSaveDetailsCommand);
            ChangePasswordCommand = new Command(ExecuteChangePasswordCommand);
            SignOutCommand = new Command(ExecuteSignOutCommand);
            ToggleCurrentPasswordVisibilityCommand = new Command(ExecuteToggleCurrentPasswordVisibility);
            ToggleNewPasswordVisibilityCommand = new Command(ExecuteToggleNewPasswordVisibility);
            ToggleConfirmNewPasswordVisibilityCommand = new Command(ExecuteToggleConfirmNewPasswordVisibility);
            TakePhotoCommand = new Command(ExecuteTakePhotoCommand);
            ChoosePhotoCommand = new Command(ExecuteChoosePhotoCommand);
            RemovePhotoCommand = new Command(ExecuteRemovePhotoCommand);
            SubmitPhotoCommand = new Command(ExecuteSubmitPhotoCommand);
        }

        // CanSaveDetails / CanChangePassword / CanSubmitPhoto all depend on IsBusy (base).
        // Local setters raise their CanXxx directly; IsBusy is raised from the base, so we re-fire here.
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(IsBusy))
            {
                OnPropertyChanged(nameof(CanSaveDetails));
                OnPropertyChanged(nameof(CanChangePassword));
                OnPropertyChanged(nameof(CanSubmitPhoto));
            }
        }

        // Called from Settings.xaml.cs OnAppearing. Pulls the current user and populates the editable fields
        // + the read-only birthday label. Load failure surfaces in the shared Errors (top of page in the photo card).
        public async void LoadDataAsync()
        {
            try
            {
                Errors = new List<string>();
                IsBusy = true;

                ResultModel<UserItem> result = await _userService.GetMeAsync();

                if (!result.IsSuccess || result.Data is null)
                {
                    Errors = result.Errors;
                    IsBusy = false;
                    return;
                }

                PopulateFromUserItem(result.Data);
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsViewModel.LoadData] {ex}");
                Errors = new List<string> { "Could not load your profile. Please try again." };
                IsBusy = false;
            }
        }

        private void PopulateFromUserItem(UserItem user)
        {
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            SelectedGender = user.Gender;
            BirthDateDisplay = user.BirthDate.ToString("dd MMM yyyy");

            // PopulateFromUserItem fires both from initial load AND from a successful SaveDetails.
            // The field setters above flipped DetailsSaved to false as a side-effect; that's the right behaviour
            // on initial load (no save just happened) but wrong on post-save refresh. The save handler re-sets
            // DetailsSaved = true after calling this method.
        }

        // ============================================================
        // === SaveDetails ===
        // ============================================================

        private async void ExecuteSaveDetailsCommand()
        {
            try
            {
                ResetDetailsErrors();

                SettingsData form = new SettingsData
                {
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Gender = SelectedGender
                };

                ValidationResult validationResult = _settingsValidator.Validate(form);

                if (!validationResult.IsValid)
                {
                    DispatchSettingsValidationFailures(validationResult);
                    return;
                }

                IsBusy = true;

                UserUpdateRequestDto request = new UserUpdateRequestDto
                {
                    Email = Email,
                    FirstName = FirstName,
                    LastName = LastName,
                    Gender = SelectedGender!.Value
                };

                ResultModel<UserItem> result = await _userService.UpdateMeAsync(request);

                if (!result.IsSuccess || result.Data is null)
                {
                    DetailsApiError = string.Join("\n", result.Errors);
                    IsBusy = false;
                    return;
                }

                PopulateFromUserItem(result.Data);
                DetailsSaved = true;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsViewModel.SaveDetails] {ex}");
                DetailsApiError = "Something went wrong. Please try again.";
                IsBusy = false;
            }
        }

        private void DispatchSettingsValidationFailures(ValidationResult validationResult)
        {
            foreach (ValidationFailure failure in validationResult.Errors)
            {
                if (failure.PropertyName == nameof(SettingsData.Email))
                {
                    EmailError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SettingsData.FirstName))
                {
                    FirstNameError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SettingsData.LastName))
                {
                    LastNameError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(SettingsData.Gender))
                {
                    GenderError = failure.ErrorMessage;
                }
                else
                {
                    // Drift-catcher: if a new RuleFor lands in the validator without a matching VM slot, fail loudly.
                    throw new NotImplementedException(
                        $"The property {failure.PropertyName} is not handled in the viewmodel");
                }
            }
        }

        private void ResetDetailsErrors()
        {
            EmailError = string.Empty;
            FirstNameError = string.Empty;
            LastNameError = string.Empty;
            GenderError = string.Empty;
            DetailsApiError = string.Empty;
            DetailsSaved = false;
        }

        // ============================================================
        // === ChangePassword ===
        // ============================================================

        private async void ExecuteChangePasswordCommand()
        {
            try
            {
                ResetPasswordErrors();

                ChangeMyPasswordData form = new ChangeMyPasswordData
                {
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword,
                    ConfirmNewPassword = ConfirmNewPassword
                };

                ValidationResult validationResult = _changeMyPasswordValidator.Validate(form);

                if (!validationResult.IsValid)
                {
                    DispatchChangePasswordValidationFailures(validationResult);
                    return;
                }

                IsBusy = true;

                ChangeMyPasswordRequestDto request = new ChangeMyPasswordRequestDto
                {
                    CurrentPassword = CurrentPassword,
                    NewPassword = NewPassword,
                    ConfirmNewPassword = ConfirmNewPassword
                };

                BaseResultModel result = await _userService.ChangeMyPasswordAsync(request);

                if (!result.IsSuccess)
                {
                    PasswordApiError = string.Join("\n", result.Errors);
                    IsBusy = false;
                    return;
                }

                // Success: clear the three password fields and flip the success indicator.
                // Setters of these fields clear PasswordSuccess on edit, so the badge auto-dismisses.
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmNewPassword = string.Empty;
                PasswordSuccess = true;
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsViewModel.ChangePassword] {ex}");
                PasswordApiError = "Something went wrong. Please try again.";
                IsBusy = false;
            }
        }

        private void DispatchChangePasswordValidationFailures(ValidationResult validationResult)
        {
            foreach (ValidationFailure failure in validationResult.Errors)
            {
                if (failure.PropertyName == nameof(ChangeMyPasswordData.CurrentPassword))
                {
                    CurrentPasswordError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(ChangeMyPasswordData.NewPassword))
                {
                    NewPasswordError = failure.ErrorMessage;
                }
                else if (failure.PropertyName == nameof(ChangeMyPasswordData.ConfirmNewPassword))
                {
                    ConfirmNewPasswordError = failure.ErrorMessage;
                }
                else
                {
                    throw new NotImplementedException(
                        $"The property {failure.PropertyName} is not handled in the viewmodel");
                }
            }
        }

        private void ResetPasswordErrors()
        {
            CurrentPasswordError = string.Empty;
            NewPasswordError = string.Empty;
            ConfirmNewPasswordError = string.Empty;
            PasswordApiError = string.Empty;
            PasswordSuccess = false;
        }

        // ============================================================
        // === SignOut ===
        // ============================================================

        private async void ExecuteSignOutCommand()
        {
            try
            {
                await _sessionService.SignOutAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsViewModel.SignOut] {ex}");
                Errors = new List<string> { "Could not sign out. Please try again." };
            }
        }

        // ============================================================
        // === Password visibility toggles ===
        // ============================================================

        private void ExecuteToggleCurrentPasswordVisibility()
        {
            IsCurrentPasswordVisible = !IsCurrentPasswordVisible;
        }

        private void ExecuteToggleNewPasswordVisibility()
        {
            IsNewPasswordVisible = !IsNewPasswordVisible;
        }

        private void ExecuteToggleConfirmNewPasswordVisibility()
        {
            IsConfirmNewPasswordVisible = !IsConfirmNewPasswordVisible;
        }

        // ============================================================
        // === Photo (unchanged from Phase 8) ===
        // ============================================================

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
            catch (Exception ex)
            {
                Debug.WriteLine($"[SettingsViewModel.TakePhoto] {ex}");
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

        private async void ExecuteSubmitPhotoCommand()
        {
            try
            {
                Errors = new List<string>();
                IsBusy = true;

                BaseResultModel result = await _userService.UpdateProfileImageAsync(
                    pickedPhotoBytes!,
                    pickedPhotoContentType!,
                    pickedPhotoFileName!);

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                    IsBusy = false;
                    return;
                }

                // Success — clear local state. User stays on Settings; next page that reads HasProfileImage will see it.
                ClearPickedPhoto();

                IsBusy = false;
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
                IsBusy = false;
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

        private void ClearPickedPhoto()
        {
            PickedPhotoBytes = null;
            PickedPhotoSource = null;
            pickedPhotoContentType = null;
            pickedPhotoFileName = null;
            PhotoError = string.Empty;
        }
    }
}
