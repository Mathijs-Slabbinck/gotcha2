using CommunityToolkit.Mvvm.ComponentModel;

namespace Gotcha2.Maui.ViewModels.BaseViewModels
{
    // Shared base for every page-bound ViewModel. Owns the universal IsBusy + ErrorMessage state.
    // Bind IsEnabled="{Binding IsBusy, Converter={StaticResource InvertBool}}" on form controls to disable
    // them during async work, and surface ErrorMessage in a styled label.
    public abstract class PageBaseViewModel : ObservableObject
    {
        private bool isBusy;
        private string errorMessage = string.Empty;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }
    }
}
