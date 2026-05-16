using CommunityToolkit.Mvvm.ComponentModel;

namespace Gotcha2.Maui.ViewModels.BaseViewModels
{
    /* Shared base for every page-bound ViewModel. Owns the universal IsBusy + Errors state.
     * Bind IsEnabled="{Binding IsBusy, Converter={StaticResource InvertBoolConverter}}" on form controls
     * to disable them during async work. For errors: bind a Label's Text to ErrorText and IsVisible to HasErrors,
     * or a CollectionView's ItemsSource to Errors for one-line-per-error rendering.
     * Errors mirrors the wire shape — IList<string> matches ResultModel.Errors so VMs can assign directly. */
    public abstract class PageBaseViewModel : ObservableObject
    {
        private bool isBusy;
        private IList<string> errors = new List<string>();

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public IList<string> Errors
        {
            get { return errors; }
            set
            {
                SetProperty(ref errors, value);
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(ErrorText));
            }
        }

        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        public string ErrorText
        {
            get { return string.Join("\n", errors); }
        }
    }
}
