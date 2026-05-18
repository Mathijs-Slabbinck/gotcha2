using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class Games : ContentPage
    {
        private readonly GamesViewModel _viewModel;

        public Games(GamesViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Games is mounted as <ShellContent> — Shell instantiates the page once and keeps it
        // alive across tab switches. Loading in the ctor would only fire once, missing post-action
        // refreshes (e.g. returning from PlayerHome after a kill). OnAppearing fires on every tab-back.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadDataAsync();
        }
    }
}
