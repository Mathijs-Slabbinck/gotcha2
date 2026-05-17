using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class Settings : ContentPage
    {
        private readonly SettingsViewModel _viewModel;

        public Settings(SettingsViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        // Settings is mounted as <ShellContent> — Shell instantiates the page once and keeps it alive
        // across tab switches. Loading in the ctor would only fire once, missing profile-edit refreshes
        // made on a different device. OnAppearing fires on every tab-back.
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadDataAsync();
        }
    }
}
