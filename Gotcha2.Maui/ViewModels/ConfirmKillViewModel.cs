using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.ComponentModel;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    /* --- comment ---
     * QueryProperty attribute allows passing parameters via Shell navigation.
     * In this case, the GameId is passed as a string query parameter and parsed into a Guid. */
    [QueryProperty(nameof(GameIdRaw), "gameId")]
    public class ConfirmKillViewModel : PageBaseViewModel
    {
        private readonly IGameService _gameService;
        private readonly IUserService _userService;
        private readonly SessionService _sessionService;

        public Guid GameId { get; private set; }

        public string GameIdRaw
        {
            set
            {
                if (Guid.TryParse(value, out Guid parsed))
                    GameId = parsed;
                else
                    Errors = new List<string> { "Invalid game id." };
            }
        }

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private TargetAssignmentItem? target;
        public TargetAssignmentItem? Target
        {
            get { return target; }
            set
            {
                SetProperty(ref target, value);
                OnPropertyChanged(nameof(HasTarget));
                OnPropertyChanged(nameof(CanReportTargetKill));
            }
        }

        public bool HasTarget
        {
            get { return target is not null; }
        }

        private ImageSource? targetImage;
        public ImageSource? TargetImage
        {
            get { return targetImage; }
            set { SetProperty(ref targetImage, value); }
        }

        private Guid? myPlayerId;
        public Guid? MyPlayerId
        {
            get { return myPlayerId; }
            set
            {
                SetProperty(ref myPlayerId, value);
                OnPropertyChanged(nameof(CanReportSelfKill));
            }
        }

        private bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set
            {
                SetProperty(ref isAlive, value);
                OnPropertyChanged(nameof(CanReportTargetKill));
                OnPropertyChanged(nameof(CanReportSelfKill));
            }
        }

        public bool CanReportTargetKill
        {
            get { return isAlive && target is not null && !IsBusy; }
        }

        public bool CanReportSelfKill
        {
            get { return isAlive && myPlayerId.HasValue && !IsBusy; }
        }

        public ICommand ReportTargetKillCommand { get; }
        public ICommand ReportSelfKillCommand { get; }

        public ConfirmKillViewModel(IGameService gameService, IUserService userService, SessionService sessionService)
        {
            _gameService = gameService;
            _userService = userService;
            _sessionService = sessionService;

            ReportTargetKillCommand = new Command(ExecuteReportTargetKillCommand);
            ReportSelfKillCommand = new Command(ExecuteReportSelfKillCommand);
        }

        /* --- comment ---
         * CanReportTargetKill / CanReportSelfKill both depend on the base IsBusy property.
         * IsBusy isn't virtual (so can't simply override it);
         * Instead we re-fire the computed props from the OnPropertyChanged override. */
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(IsBusy))
            {
                OnPropertyChanged(nameof(CanReportTargetKill));
                OnPropertyChanged(nameof(CanReportSelfKill));
            }
        }

        public async void LoadDataAsync()
        {
            try
            {
                Errors = new List<string>();
                IsBusy = true;

                if (GameId == Guid.Empty)
                {
                    Errors = new List<string> { "Invalid game id." };
                    return;
                }

                Task<ResultModel<GameDetailItem>> gameTask = _gameService.GetByIdAsync(GameId);
                Task<ResultModel<TargetAssignmentItem?>> targetTask = _gameService.GetMyTargetAsync(GameId);

                await Task.WhenAll(gameTask, targetTask);

                ResultModel<GameDetailItem> gameResult = gameTask.Result;
                ResultModel<TargetAssignmentItem?> targetResult = targetTask.Result;

                if (!gameResult.IsSuccess)
                {
                    Errors = gameResult.Errors;
                    return;
                }

                if (gameResult.Data is null)
                {
                    Errors = new List<string> { "Empty response from server." };
                    return;
                }

                if (!targetResult.IsSuccess)
                {
                    Errors = targetResult.Errors;
                    return;
                }

                GameDetailItem game = gameResult.Data;
                Title = game.Name;

                Guid currentUserId = _sessionService.CurrentUserId ?? Guid.Empty;
                PlayerItem? me = game.Players.FirstOrDefault(p => p.UserId == currentUserId);

                if (me is null)
                {
                    MyPlayerId = null;
                    IsAlive = false;
                }
                else
                {
                    MyPlayerId = me.PlayerId;
                    IsAlive = me.IsAlive;
                }

                Target = targetResult.Data;

                await LoadTargetImageAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConfirmKillViewModel.LoadDataAsync failed: {ex.Message}");
                Errors = new List<string> { "Something went wrong. Please try again." };
            }

            IsBusy = false;
        }

        private async Task LoadTargetImageAsync()
        {
            if (Target is null)
            {
                TargetImage = null;
                return;
            }

            if (!Target.TargetHasProfileImage)
            {
                TargetImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
                return;
            }

            ResultModel<byte[]> imageResult = await _userService.GetProfileImageAsync(Target.TargetUserId);

            if (imageResult.IsSuccess && imageResult.Data is not null)
            {
                byte[] bytes = imageResult.Data;
                TargetImage = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            else
            {
                // Silent fallback — image fetch failure shouldn't block the report buttons.
                TargetImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
            }
        }

        private async void ExecuteReportTargetKillCommand()
        {
            try
            {
                if (target is null)
                    return;

                Errors = new List<string>();
                IsBusy = true;

                KillRequestDto request = new KillRequestDto { VictimPlayerId = target.TargetPlayerId };
                ResultModel<KillItem> result = await _gameService.RecordKillAsync(GameId, request);

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                    IsBusy = false;
                    return;
                }

                // Pop back to PlayerHome; its OnAppearing refreshes the new target.
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConfirmKillViewModel.ExecuteReportTargetKillCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not record the kill. Please try again." };
                IsBusy = false;
            }
        }

        private async void ExecuteReportSelfKillCommand()
        {
            try
            {
                if (!myPlayerId.HasValue)
                    return;

                Errors = new List<string>();
                IsBusy = true;

                KillRequestDto request = new KillRequestDto { VictimPlayerId = myPlayerId.Value };
                ResultModel<KillItem> result = await _gameService.RecordKillAsync(GameId, request);

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                    IsBusy = false;
                    return;
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ConfirmKillViewModel.ExecuteReportSelfKillCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not record the kill. Please try again." };
                IsBusy = false;
            }
        }
    }
}
