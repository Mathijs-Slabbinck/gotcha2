using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class Home : ContentPage
    {
        private readonly HomeViewModel _viewModel;

        public Home(HomeViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Home is mounted as <ShellContent> — Shell instantiates the page once and keeps it
        // alive across tab switches. Loading in the ctor would only fire once, missing post-game
        // stat refreshes. OnAppearing fires on every tab-back.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadData();
        }
    }
}
