using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class GamesViewModel : PageBaseViewModel
    {
        private readonly IGameService _gameService;

        private IList<GameItem> pendingGames = new List<GameItem>();
        public IList<GameItem> PendingGames
        {
            get { return pendingGames; }
            set
            {
                SetProperty(ref pendingGames, value);
                OnPropertyChanged(nameof(HasPending));
            }
        }

        private IList<GameItem> activeGames = new List<GameItem>();
        public IList<GameItem> ActiveGames
        {
            get { return activeGames; }
            set
            {
                SetProperty(ref activeGames, value);
                OnPropertyChanged(nameof(HasActive));
            }
        }

        private IList<GameItem> pastGames = new List<GameItem>();
        public IList<GameItem> PastGames
        {
            get { return pastGames; }
            set
            {
                SetProperty(ref pastGames, value);
                OnPropertyChanged(nameof(HasPast));
            }
        }

        public bool HasPending
        {
            get { return pendingGames.Count > 0; }
        }

        public bool HasActive
        {
            get { return activeGames.Count > 0; }
        }

        public bool HasPast
        {
            get { return pastGames.Count > 0; }
        }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public ICommand OpenGameCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand RefreshCommand { get; }

        public GamesViewModel(IGameService gameService)
        {
            _gameService = gameService;

            OpenGameCommand = new Command<GameItem>(ExecuteOpenGameCommand);
            NewGameCommand = new Command(ExecuteNewGameCommand);
            RefreshCommand = new Command(ExecuteRefreshCommand);
        }

        public async void LoadDataAsync()
        {
            try
            {
                Errors = new List<string>();
                IsBusy = true;

                ResultModel<List<GameItem>> result = await _gameService.GetAllAsync();

                if (!result.IsSuccess)
                {
                    Errors = result.Errors;
                    return;
                }

                if (result.Data is null)
                {
                    Errors = new List<string> { "Empty response from server." };
                    return;
                }

                List<GameItem> pending = new List<GameItem>();
                List<GameItem> active = new List<GameItem>();
                List<GameItem> past = new List<GameItem>();

                foreach (GameItem game in result.Data)
                {
                    if (game.IsFinished)
                    {
                        past.Add(game);
                    }
                    else if (game.HasStarted)
                    {
                        active.Add(game);
                    }
                    else
                    {
                        pending.Add(game);
                    }
                }

                PendingGames = pending;
                ActiveGames = active;
                PastGames = past;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GamesViewModel.LoadDataAsync failed: {ex.Message}");
                Errors = new List<string> { "Something went wrong. Please try again." };
            }

            IsBusy = false;
            IsRefreshing = false;
        }

        private async void ExecuteOpenGameCommand(GameItem game)
        {
            try
            {
                if (game is null)
                    return;

                await Shell.Current.GoToAsync($"{RoutesConstants.PlayerHome}?gameId={game.GameId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GamesViewModel.ExecuteOpenGameCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not open the game. Please try again." };
            }
        }

        private async void ExecuteNewGameCommand()
        {
            try
            {
                await Shell.Current.GoToAsync(RoutesConstants.NewGame);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GamesViewModel.ExecuteNewGameCommand failed: {ex.Message}");
                Errors = new List<string> { "Could not open the new-game page. Please try again." };
            }
        }

        private void ExecuteRefreshCommand()
        {
            LoadDataAsync();
        }
    }
}
