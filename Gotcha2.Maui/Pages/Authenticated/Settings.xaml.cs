using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class Settings : ContentPage
    {
        public Settings(SettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
