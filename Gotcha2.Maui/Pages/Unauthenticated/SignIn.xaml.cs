using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Unauthenticated
{
    // partial because the XAML compiler generates a sibling partial that defines the named children (EmailEntry, PasswordEntry, etc.)
    // plus the InitializeComponent() method.
    // Your hand-written half + the generated half merge into one class at compile time
    public partial class SignIn : ContentPage
    {
        public SignIn(SignInViewModel viewModel)
        {
            InitializeComponent();

            // wire the View to the ViewModel
            BindingContext = viewModel;
        }
    }
}
