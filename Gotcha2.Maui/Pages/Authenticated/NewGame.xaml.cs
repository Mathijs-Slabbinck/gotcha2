using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class NewGame : ContentPage
    {
        public NewGame(NewGameViewModel viewModel)
        {
            InitializeComponent();

            // wire the View to the ViewModel
            BindingContext = viewModel;
        }
    }
}
