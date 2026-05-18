using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class PlayerHome : ContentPage
    {
        private readonly PlayerHomeViewModel _viewModel;

        public PlayerHome(PlayerHomeViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /* --- comment ---
         * PlayerHome is pushed onto the Games tab nav stack
         * Shell instantiates the page every time, so [QueryProperty] on the VM has GameId set by the time OnAppearing fires.
         * OnAppearing (not the ctor) is also how we refresh when the user navigates back from ConfirmKill. */
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadDataAsync();
        }
    }
}
