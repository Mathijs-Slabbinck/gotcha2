using Gotcha2.Maui.Constants;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;
using Gotcha2.Maui.Services;
using Gotcha2.Maui.ViewModels.BaseViewModels;
using System.Windows.Input;

namespace Gotcha2.Maui.ViewModels
{
    public class NewGameViewModel : PageBaseViewModel
    {
        private const int MaxNameLength = 100;

        private readonly IGameService _gameService;

        private string name = string.Empty;
        public string Name
        {
            get { return name; }
            set
            {
                SetProperty(ref name, value);
                NameError = string.Empty;
            }
        }

        private string nameError = string.Empty;
        public string NameError
        {
            get { return nameError; }
            set { SetProperty(ref nameError, value); }
        }

        public ICommand CreateCommand { get; }

        public NewGameViewModel(IGameService gameService)
        {
            _gameService = gameService;

            CreateCommand = new Command(ExecuteCreateCommand);
        }

        private async void ExecuteCreateCommand()
        {
            try
            {
                ResetErrors();

                string trimmedName = Name?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(trimmedName))
                    NameError = "Name is required.";
                else if (trimmedName.Length > MaxNameLength)
                    NameError = $"Name is too long (max {MaxNameLength} characters).";

                if (!string.IsNullOrEmpty(NameError))
                    return;

                IsBusy = true;

                GameRequestDto request = new GameRequestDto { Name = trimmedName };
                ResultModel<GameItem> result = await _gameService.CreateAsync(request);

                if (!result.IsSuccess || result.Data is null)
                {
                    Errors = result.Errors;
                    return;
                }

                /* --- comment ---
                 * Replace NewGame in the nav stack with PlayerHome (Pending state)
                 * so the back button returns the user to Games rather than to this now-empty form. */
                await Shell.Current.GoToAsync($"../{RoutesConstants.PlayerHome}?gameId={result.Data.GameId}");
            }
            catch
            {
                Errors = new List<string> { "Something went wrong. Please try again." };
            }

            IsBusy = false;
        }

        private void ResetErrors()
        {
            NameError = string.Empty;
            Errors = new List<string>();
        }
    }
}