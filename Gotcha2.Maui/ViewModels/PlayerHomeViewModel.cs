using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Enums;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    /* QueryProperty attribute allows passing parameters via Shell navigation.
     * In this case, the GameId is passed as a string query parameter and parsed into a Guid.
     * We chose this because the alternative would be harder to read */
    [QueryProperty(nameof(GameIdRaw), "gameId")]

    public class PlayerHomeViewModel : PageBaseViewModel
    {
        private readonly IGameService _gameService;
        private readonly IUserService _userService;
        private readonly SessionService _sessionService;

        private GameDetailItem? gameDetail;

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

        // --- Common state ---

        private string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private PlayerHomeState state = PlayerHomeState.Loading;
        public PlayerHomeState State
        {
            get { return state; }
            set
            {
                SetProperty(ref state, value);
                OnPropertyChanged(nameof(IsPending));
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(IsPast));
            }
        }

        public bool IsPending
        {
            get { return state == PlayerHomeState.Pending; }
        }

        public bool IsActive
        {
            get { return state == PlayerHomeState.Active; }
        }

        public bool IsPast
        {
            get { return state == PlayerHomeState.Past; }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        // --- Pending state ---

        private IList<PlayerItem> lobbyPlayers = new List<PlayerItem>();
        public IList<PlayerItem> LobbyPlayers
        {
            get { return lobbyPlayers; }
            set
            {
                SetProperty(ref lobbyPlayers, value);
                OnPropertyChanged(nameof(HasLobbyPlayers));
            }
        }

        public bool HasLobbyPlayers
        {
            get { return lobbyPlayers.Count > 0; }
        }

        private bool isCreator;
        public bool IsCreator
        {
            get { return isCreator; }
            set { SetProperty(ref isCreator, value); }
        }

        private Guid? myPlayerId;
        public Guid? MyPlayerId
        {
            get { return myPlayerId; }
            set
            {
                SetProperty(ref myPlayerId, value);
                OnPropertyChanged(nameof(CanLeave));
            }
        }

        public bool CanLeave
        {
            get { return myPlayerId.HasValue && state == PlayerHomeState.Pending; }
        }

        // --- Active state ---

        private ImageSource? myImage;
        public ImageSource? MyImage
        {
            get { return myImage; }
            set { SetProperty(ref myImage, value); }
        }

        private string myDisplayName = string.Empty;
        public string MyDisplayName
        {
            get { return myDisplayName; }
            set { SetProperty(ref myDisplayName, value); }
        }

        private bool isAlive;
        public bool IsAlive
        {
            get { return isAlive; }
            set { SetProperty(ref isAlive, value); }
        }

        private TargetAssignmentItem? target;
        public TargetAssignmentItem? Target
        {
            get { return target; }
            set
            {
                SetProperty(ref target, value);
                OnPropertyChanged(nameof(HasTarget));
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

        private string? killedByName;
        public string? KilledByName
        {
            get { return killedByName; }
            set { SetProperty(ref killedByName, value); }
        }

        private IList<KillItem> myKills = new List<KillItem>();
        public IList<KillItem> MyKills
        {
            get { return myKills; }
            set
            {
                SetProperty(ref myKills, value);
                OnPropertyChanged(nameof(HasMyKills));
            }
        }

        public bool HasMyKills
        {
            get { return myKills.Count > 0; }
        }

        // --- Past state ---

        private string winnerDisplayName = string.Empty;
        public string WinnerDisplayName
        {
            get { return winnerDisplayName; }
            set { SetProperty(ref winnerDisplayName, value); }
        }

        private ImageSource? winnerImage;
        public ImageSource? WinnerImage
        {
            get { return winnerImage; }
            set { SetProperty(ref winnerImage, value); }
        }

        private IList<PastPlayerRow> playerStandings = new List<PastPlayerRow>();
        public IList<PastPlayerRow> PlayerStandings
        {
            get { return playerStandings; }
            set { SetProperty(ref playerStandings, value); }
        }

        private IList<KillItem> allKills = new List<KillItem>();
        public IList<KillItem> AllKills
        {
            get { return allKills; }
            set { SetProperty(ref allKills, value); }
        }

        private IList<KillItem> filteredKills = new List<KillItem>();
        public IList<KillItem> FilteredKills
        {
            get { return filteredKills; }
            set
            {
                SetProperty(ref filteredKills, value);
                OnPropertyChanged(nameof(HasFilteredKills));
            }
        }

        public bool HasFilteredKills
        {
            get { return filteredKills.Count > 0; }
        }

        private Guid? selectedFilterPlayerId;
        public Guid? SelectedFilterPlayerId
        {
            get { return selectedFilterPlayerId; }
            set { SetProperty(ref selectedFilterPlayerId, value); }
        }

        private string filterDisplayText = string.Empty;
        public string FilterDisplayText
        {
            get { return filterDisplayText; }
            set { SetProperty(ref filterDisplayText, value); }
        }

        // --- Commands ---

        public ICommand RefreshCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand LeaveCommand { get; }
        public ICommand ShareInviteCommand { get; }
        public ICommand ConfirmKillCommand { get; }
        public ICommand FilterByPlayerCommand { get; }
        public ICommand ShareStandingsCommand { get; }

        public PlayerHomeViewModel(IGameService gameService, IUserService userService, SessionService sessionService)
        {
            _gameService = gameService;
            _userService = userService;
            _sessionService = sessionService;

            RefreshCommand = new Command(ExecuteRefreshCommand);
            StartGameCommand = new Command(ExecuteStartGameCommand);
            LeaveCommand = new Command(ExecuteLeaveCommand);
            ShareInviteCommand = new Command(ExecuteShareInviteCommand);
            ConfirmKillCommand = new Command(ExecuteConfirmKillCommand);
            FilterByPlayerCommand = new Command<PastPlayerRow>(ExecuteFilterByPlayerCommand);
            ShareStandingsCommand = new Command(ExecuteShareStandingsCommand);
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

                ResultModel<GameDetailItem> gameResult = await _gameService.GetByIdAsync(GameId);

                if (!gameResult.IsSuccess)
                {
                    Errors = gameResult.Errors;
                    State = PlayerHomeState.Loading;
                    return;
                }

                if (gameResult.Data is null)
                {
                    Errors = new List<string> { "Empty response from server." };
                    State = PlayerHomeState.Loading;
                    return;
                }

                gameDetail = gameResult.Data;

                Title = gameDetail.Name;

                if (gameDetail.IsPending)
                {
                    State = PlayerHomeState.Pending;
                    await LoadPendingStateAsync();
                }
                else if (gameDetail.IsActive)
                {
                    State = PlayerHomeState.Active;
                    await LoadActiveStateAsync();
                }
                else
                {
                    State = PlayerHomeState.Past;
                    await LoadPastStateAsync();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayerHomeViewModel.LoadDataAsync failed: {ex.Message}");
                Errors = new List<string> { "Something went wrong. Please try again." };
            }

            IsBusy = false;
            IsRefreshing = false;
        }

        private Task LoadPendingStateAsync()
        {
            if (gameDetail is null)
                return Task.CompletedTask;

            LobbyPlayers = gameDetail.Players;
            IsCreator = gameDetail.IsCreator;
            MyPlayerId = gameDetail.CurrentUserPlayerId;
            // Force CanLeave to re-evaluate after State has flipped to Pending.
            OnPropertyChanged(nameof(CanLeave));

            return Task.CompletedTask;
        }

        private async Task LoadActiveStateAsync()
        {
            if (gameDetail is null)
                return;

            Task<ResultModel<List<KillItem>>> killsTask = _gameService.GetKillsAsync(GameId);
            Task<ResultModel<TargetAssignmentItem?>> targetTask = _gameService.GetMyTargetAsync(GameId);

            await Task.WhenAll(killsTask, targetTask);

            ResultModel<List<KillItem>> killsResult = killsTask.Result;
            ResultModel<TargetAssignmentItem?> targetResult = targetTask.Result;

            if (!killsResult.IsSuccess)
            {
                Errors = killsResult.Errors;
                return;
            }

            if (!targetResult.IsSuccess)
            {
                Errors = targetResult.Errors;
                return;
            }

            AllKills = killsResult.Data ?? new List<KillItem>();
            Target = targetResult.Data;

            Guid currentUserId = _sessionService.CurrentUserId ?? Guid.Empty;

            PlayerItem? me = gameDetail.Players.FirstOrDefault(p => p.UserId == currentUserId);

            if (me is not null)
            {
                MyDisplayName = me.DisplayName;
                IsAlive = me.IsAlive;
            }
            else
            {
                MyDisplayName = string.Empty;
                IsAlive = false;
            }

            MyKills = AllKills.Where(k => k.KillerUserId == currentUserId).ToList();

            if (!IsAlive)
            {
                KillItem? killOfMe = AllKills.FirstOrDefault(k => k.VictimUserId == currentUserId);
                KilledByName = killOfMe?.KillerDisplayName;
            }
            else
            {
                KilledByName = null;
            }

            // Always fetch my image first, then the target's image if there is one.
            await LoadMyImageAsync(me);
            await LoadTargetImageAsync();
        }

        private async Task LoadMyImageAsync(PlayerItem? me)
        {
            if (me is null || !me.HasProfileImage)
            {
                MyImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
                return;
            }

            ResultModel<byte[]> imageResult = await _userService.GetProfileImageAsync(me.UserId);

            if (imageResult.IsSuccess && imageResult.Data is not null)
            {
                byte[] bytes = imageResult.Data;
                MyImage = ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            else
            {
                // Silent fallback — image fetch failure shouldn't block the active view.
                MyImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
            }
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
                TargetImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
            }
        }

        private async Task LoadPastStateAsync()
        {
            if (gameDetail is null)
                return;

            ResultModel<List<KillItem>> killsResult = await _gameService.GetKillsAsync(GameId);

            if (!killsResult.IsSuccess)
            {
                Errors = killsResult.Errors;
                return;
            }

            AllKills = killsResult.Data ?? new List<KillItem>();

            PlayerItem? winner = gameDetail.Players.FirstOrDefault(p => p.UserId == gameDetail.WinnerId);
            WinnerDisplayName = winner?.DisplayName ?? "Unknown";

            List<PastPlayerRow> rows = new List<PastPlayerRow>();

            foreach (PlayerItem player in gameDetail.Players)
            {
                bool playerIsWinner = player.UserId == gameDetail.WinnerId;
                string status;

                if (playerIsWinner)
                {
                    status = "Winner";
                }
                else
                {
                    KillItem? killOfPlayer = AllKills.FirstOrDefault(k => k.VictimUserId == player.UserId);

                    if (killOfPlayer is not null)
                    {
                        status = $"Killed by {killOfPlayer.KillerDisplayName}";
                    }
                    else
                    {
                        status = "Eliminated";
                    }
                }

                rows.Add(new PastPlayerRow
                {
                    PlayerId = player.PlayerId,
                    UserId = player.UserId,
                    DisplayName = player.DisplayName,
                    IsWinner = playerIsWinner,
                    Status = status,
                    HasProfileImage = player.HasProfileImage
                });
            }

            PlayerStandings = rows;

            // Default filter: winner's kills.
            SelectedFilterPlayerId = null;
            ApplyKillFilter();

            // Winner image.
            if (winner is not null && winner.HasProfileImage)
            {
                ResultModel<byte[]> imageResult = await _userService.GetProfileImageAsync(winner.UserId);

                if (imageResult.IsSuccess && imageResult.Data is not null)
                {
                    byte[] bytes = imageResult.Data;
                    WinnerImage = ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                else
                {
                    WinnerImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
                }
            }
            else
            {
                WinnerImage = ImageSource.FromFile(DevConstants.ProfilePlaceholderFileName);
            }
        }

        private void ApplyKillFilter()
        {
            if (gameDetail is null)
            {
                FilteredKills = new List<KillItem>();
                FilterDisplayText = string.Empty;
                return;
            }

            Guid filterUserId;
            string filterName;

            if (selectedFilterPlayerId.HasValue)
            {
                filterUserId = selectedFilterPlayerId.Value;
                PastPlayerRow? row = playerStandings.FirstOrDefault(r => r.UserId == filterUserId);
                filterName = row?.DisplayName ?? "player";
            }
            else
            {
                // No filter selected — default to the winner.
                filterUserId = gameDetail.WinnerId ?? Guid.Empty;
                filterName = winnerDisplayName;
            }

            FilteredKills = allKills.Where(k => k.KillerUserId == filterUserId).ToList();
            FilterDisplayText = $"Showing kills by {filterName}";
        }

        private void ExecuteRefreshCommand()
        {
            LoadDataAsync();
        }

        private async void ExecuteStartGameCommand()
        {
            try
            {
                Errors = new List<string>();
                IsBusy = true;

                BaseResultModel result = await _gameService.StartAsync(GameId);

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                    IsBusy = false;
                    return;
                }

                // IsBusy left true here; LoadDataAsync will reset it once the refreshed game lands.
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayerHomeViewModel.ExecuteStartGameCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not start the game. Please try again." };
                IsBusy = false;
            }
        }

        private async void ExecuteLeaveCommand()
        {
            try
            {
                if (!MyPlayerId.HasValue)
                    return;

                Errors = new List<string>();
                IsBusy = true;

                BaseResultModel result = await _gameService.LeaveAsync(MyPlayerId.Value);

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                    IsBusy = false;
                    return;
                }

                await Shell.Current.GoToAsync(RoutesConstants.Games);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayerHomeViewModel.ExecuteLeaveCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not leave the game. Please try again." };
                IsBusy = false;
            }
        }

        private async void ExecuteShareInviteCommand()
        {
            try
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = "Gotcha invite",
                    Text = $"Join my Gotcha game \"{Title}\"! Game ID: {GameId}"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayerHomeViewModel.ExecuteShareInviteCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not open the share dialog." };
            }
        }

        private async void ExecuteConfirmKillCommand()
        {
            try
            {
                await Shell.Current.GoToAsync($"{RoutesConstants.ConfirmKill}?gameId={GameId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayerHomeViewModel.ExecuteConfirmKillCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not open the confirm-kill page. Please try again." };
            }
        }

        private void ExecuteFilterByPlayerCommand(PastPlayerRow row)
        {
            if (row is null)
                return;

            // Tap the currently-selected row to clear the filter (toggle back to default = winner).
            if (selectedFilterPlayerId.HasValue && selectedFilterPlayerId.Value == row.UserId)
                SelectedFilterPlayerId = null;
            else
                SelectedFilterPlayerId = row.UserId;

            ApplyKillFilter();
        }

        private async void ExecuteShareStandingsCommand()
        {
            try
            {
                if (gameDetail is null)
                    return;

                int winnerKillCount = 0;

                if (gameDetail.WinnerId.HasValue)
                {
                    winnerKillCount = allKills.Count(k => k.KillerUserId == gameDetail.WinnerId.Value);
                }

                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Title = "Gotcha results",
                    Text = $"{Title} is over — winner: {WinnerDisplayName} with {winnerKillCount} kills"
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PlayerHomeViewModel.ExecuteShareStandingsCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not open the share dialog." };
            }
        }
    }
}
