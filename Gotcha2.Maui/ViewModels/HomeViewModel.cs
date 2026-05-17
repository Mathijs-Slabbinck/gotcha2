using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;

namespace Gotcha2.Maui.ViewModels
{
    public class HomeViewModel : PageBaseViewModel
    {
        private readonly IUserService _userService;

        private string displayName = string.Empty;
        public string DisplayName
        {
            get { return displayName; }
            set { SetProperty(ref displayName, value); }
        }

        private int gamesPlayed;
        public int GamesPlayed
        {
            get { return gamesPlayed; }
            set { SetProperty(ref gamesPlayed, value); }
        }

        private int gamesWon;
        public int GamesWon
        {
            get { return gamesWon; }
            set { SetProperty(ref gamesWon, value); }
        }

        private int totalKills;
        public int TotalKills
        {
            get { return totalKills; }
            set { SetProperty(ref totalKills, value); }
        }

        private string accountCreatedText = string.Empty;
        public string AccountCreatedText
        {
            get { return accountCreatedText; }
            set { SetProperty(ref accountCreatedText, value); }
        }

        private ImageSource? profileImage;
        public ImageSource? ProfileImage
        {
            get { return profileImage; }
            set { SetProperty(ref profileImage, value); }
        }

        public HomeViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public async void LoadData()
        {
            try
            {
                Errors = new List<string>();
                IsBusy = true;

                ResultModel<UserItem> userResult = await _userService.GetMeAsync();

                if (!userResult.IsSuccess)
                {
                    Errors = userResult.Errors;
                    return;
                }

                if (userResult.Data is null)
                {
                    Errors = new List<string> { "Empty response from server." };
                    return;
                }

                UserItem user = userResult.Data;

                DisplayName = user.DisplayName;
                GamesPlayed = user.GamesPlayed;
                GamesWon = user.GamesWon;
                TotalKills = user.TotalKills;
                AccountCreatedText = user.AccountCreationDateText;

                if (user.HasProfileImage)
                {
                    ResultModel<byte[]> imageResult = await _userService.GetProfileImageAsync(user.UserId);

                    if (imageResult.IsSuccess && imageResult.Data is not null)
                    {
                        byte[] bytes = imageResult.Data;
                        ProfileImage = ImageSource.FromStream(() => new MemoryStream(bytes));
                    }
                    else
                    {
                        // Silent fallback — image fetch failure shouldn't block the stats view.
                        // ApiUserService already logs via Debug.WriteLine.
                        ProfileImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
                    }
                }
                else
                {
                    ProfileImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HomeViewModel.LoadData failed: {ex.Message}");
                Errors = new List<string> { "Something went wrong. Please try again." };
            }

            IsBusy = false;
        }
    }
}
