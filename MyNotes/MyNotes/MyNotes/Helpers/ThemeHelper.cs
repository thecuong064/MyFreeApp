using MyNotes;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyNotes.Helpers
{
    public static class ThemeHelper
    {
        public const int THEME_UNSPECIFIED = 0;
        public const int THEME_LIGHT = 1;
        public const int THEME_DARK = 2;

        #region SetTheme

        public static void SetTheme(OSAppTheme theme)
        {
            switch (theme)
            {
                case OSAppTheme.Unspecified:
                    AppSettings.Theme = THEME_UNSPECIFIED;
                    break;
                case OSAppTheme.Light:
                    AppSettings.Theme = THEME_LIGHT;
                    break;
                case OSAppTheme.Dark:
                    AppSettings.Theme = THEME_DARK;
                    break;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                App.Current.UserAppTheme = theme;
            });

            //var nav = App.Current.MainPage as Xamarin.Forms.NavigationPage;

            //var e = DependencyService.Get<IEnvironment>();
            //if (App.Current.RequestedTheme == OSAppTheme.Dark)
            //{
            //    e?.SetStatusBarColor(Color.Black, false);
            //    if (nav != null)
            //    {
            //        nav.BarBackgroundColor = Color.Black;
            //        nav.BarTextColor = Color.White;
            //    }
            //}
            //else
            //{
            //    e?.SetStatusBarColor(Color.White, true);
            //    if (nav != null)
            //    {
            //        nav.BarBackgroundColor = Color.White;
            //        nav.BarTextColor = Color.Black;
            //    }
            //}
        }

        #endregion

        #region SwitchTheme

        public static void SwitchTheme()
        {
            var themeToSwitch = OSAppTheme.Light;
            switch (App.Current.UserAppTheme)
            {
                case OSAppTheme.Unspecified:
                case OSAppTheme.Light:
                    themeToSwitch = OSAppTheme.Dark;
                    AppSettings.Theme = THEME_DARK;
                    break;
                case OSAppTheme.Dark:
                    AppSettings.Theme = THEME_LIGHT;
                    themeToSwitch = OSAppTheme.Light;
                    break;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                App.Current.UserAppTheme = themeToSwitch;
            });

            //var nav = App.Current.MainPage as Xamarin.Forms.NavigationPage;

            //var e = DependencyService.Get<IEnvironment>();
            //if (App.Current.RequestedTheme == OSAppTheme.Dark)
            //{
            //    e?.SetStatusBarColor(Color.Black, false);
            //    if (nav != null)
            //    {
            //        nav.BarBackgroundColor = Color.Black;
            //        nav.BarTextColor = Color.White;
            //    }
            //}
            //else
            //{
            //    e?.SetStatusBarColor(Color.White, true);
            //    if (nav != null)
            //    {
            //        nav.BarBackgroundColor = Color.White;
            //        nav.BarTextColor = Color.Black;
            //    }
            //}
        }

        #endregion

        #region SetLastChosenTheme

        public static void SetLastChosenTheme()
        {
            switch (AppSettings.Theme)
            {
                case THEME_UNSPECIFIED:
                    App.Current.UserAppTheme = OSAppTheme.Unspecified;
                    break;
                case THEME_LIGHT:
                    App.Current.UserAppTheme = OSAppTheme.Light;
                    break;
                case THEME_DARK:
                    App.Current.UserAppTheme = OSAppTheme.Dark;
                    break;
            }

            //var nav = App.Current.MainPage as Xamarin.Forms.NavigationPage;

            //var e = DependencyService.Get<IEnvironment>();
            //if (App.Current.RequestedTheme == OSAppTheme.Dark)
            //{
            //    e?.SetStatusBarColor(Color.Black, false);
            //    if (nav != null)
            //    {
            //        nav.BarBackgroundColor = Color.Black;
            //        nav.BarTextColor = Color.White;
            //    }
            //}
            //else
            //{
            //    e?.SetStatusBarColor(Color.White, true);
            //    if (nav != null)
            //    {
            //        nav.BarBackgroundColor = Color.White;
            //        nav.BarTextColor = Color.Black;
            //    }
            //}
        }

        #endregion
    }
}