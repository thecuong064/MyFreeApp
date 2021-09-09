using MvvmHelpers;
using MyNotes.Helpers;
using MyNotes.Views;
using Command = Xamarin.Forms.Command;

namespace MyNotes.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        public BasePage Page { get; set; }

        private bool isFirstTimeAppeared = true;
        public bool IsFirstTimeAppeared
        {
            get => isFirstTimeAppeared;
            set => SetProperty(ref isFirstTimeAppeared, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        #region SwitchThemeCommand

        private Command switchThemeCommand;
        public Command SwitchThemeCommand => switchThemeCommand ??= new Command(ExecuteSwitchThemeCommand);
        
        public void ExecuteSwitchThemeCommand()
        {
            ThemeHelper.SwitchTheme();
        }

        #endregion
    }
}
