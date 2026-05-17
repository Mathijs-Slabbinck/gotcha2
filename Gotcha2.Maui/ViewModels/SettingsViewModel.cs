using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.ComponentModel;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class SettingsViewModel : PageBaseViewModel
    {
        private readonly IUserService _userService;

        // Photo upload constants — mirror the API's PUT /api/users/me/profile-image gate.
        // Intentionally duplicated with SignUpViewModel; per convention §10 ("avoid over-abstracting"),
        // the two pages don't share a base. Extract to Constants/ if a third consumer lands.
        private const long MaxPhotoBytes = 5 * 1024 * 1024;
        private const string JpegMimeType = "image/jpeg";
        private const string PngMimeType = "image/png";
        private const string DefaultPhotoFileName = "photo.jpg";

        // Picked photo state — bytes + source for preview, contentType + fileName for upload.
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

        // Combined gate for the Submit Photo button — visible/enabled only when a valid photo is queued and we're idle.
        // Refreshes via the OnPropertyChanged override (on IsBusy), via PhotoError's setter, and via PickedPhotoBytes's setter.
        public bool CanSubmitPhoto
        {
            get { return !IsBusy && HasPickedPhoto && string.IsNullOrEmpty(photoError); }
        }

        public ICommand TakePhotoCommand { get; }
        public ICommand ChoosePhotoCommand { get; }
        public ICommand RemovePhotoCommand { get; }
        public ICommand SubmitPhotoCommand { get; }

        public SettingsViewModel(IUserService userService)
        {
            _userService = userService;

            TakePhotoCommand = new Command(ExecuteTakePhotoCommand);
            ChoosePhotoCommand = new Command(ExecuteChoosePhotoCommand);
            RemovePhotoCommand = new Command(ExecuteRemovePhotoCommand);
            SubmitPhotoCommand = new Command(ExecuteSubmitPhotoCommand);
        }

        // CanSubmitPhoto depends on IsBusy (base), PhotoError (local), and PickedPhotoBytes (local).
        // Local setters raise CanSubmitPhoto directly; IsBusy is raised from the base, so we re-fire here.
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(IsBusy))
                OnPropertyChanged(nameof(CanSubmitPhoto));
        }

        // TODO Phase 6: editable user fields (FirstName, LastName, Email, BirthDate, Gender) bound to IUserService.UpdateMeAsync.
        // TODO Phase 6: change-password form bound to IUserService.ChangePasswordAsync.

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
