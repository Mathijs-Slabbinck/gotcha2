using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Unauthenticated
{
    public partial class SignUp : ContentPage
    {
        public SignUp(SignUpViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
