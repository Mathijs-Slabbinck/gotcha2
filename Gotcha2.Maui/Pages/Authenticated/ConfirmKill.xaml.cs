using Gotcha2.Maui.ViewModels;

namespace Gotcha2.Maui.Pages.Authenticated
{
    public partial class ConfirmKill : ContentPage
    {
        private readonly ConfirmKillViewModel _viewModel;

        public ConfirmKill(ConfirmKillViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /* --- comment ---
         * ConfirmKill is pushed onto the Games tab nav stack from PlayerHome.
         * Shell instantiates the page every time, so [QueryProperty] on the VM has GameId set by the time OnAppearing fires.
         * OnAppearing (not the ctor) lets a return-trip from anywhere refresh the target. */
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadDataAsync();
        }
    }
}
