using CommunityToolkit.Mvvm.ComponentModel;

namespace Gotcha2.Maui.ViewModels.BaseViewModels
{
    // Shared base for every page-bound ViewModel. Owns the universal IsBusy + ErrorMessage state.
    // Bind IsEnabled="{Binding IsBusy, Converter={StaticResource InvertBool}}" on form controls to disable
    // them during async work, and surface ErrorMessage in a styled label.
    public abstract class PageBaseViewModel : ObservableObject
    {
        private bool _isBusy;
        private string _errorMessage = string.Empty;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { SetProperty(ref _errorMessage, value); }
        }
    }
}
