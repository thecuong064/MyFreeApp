using MyNotes.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyNotes
{
    public partial class App : Application
    {

        public App()
        {
            DevExpress.XamarinForms.Editors.Initializer.Init();
            DevExpress.XamarinForms.CollectionView.Initializer.Init();
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            OnResume();
        }

        protected override void OnSleep()
        {
            ThemeHelper.SetLastChosenTheme();
            //RequestedThemeChanged -= App_RequestedThemeChanged;
        }

        protected override void OnResume()
        {
            ThemeHelper.SetLastChosenTheme();
            //RequestedThemeChanged += App_RequestedThemeChanged;
        }

        private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                //ThemeHelper.SetLastChosenTheme();
            });
        }
    }
}
