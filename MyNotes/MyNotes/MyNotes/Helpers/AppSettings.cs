using Xamarin.Essentials;

namespace MyNotes.Helpers
{
    public static class AppSettings
    {
        public static int Theme
        {
            get => Preferences.Get(nameof(Theme), ThemeHelper.THEME_UNSPECIFIED);
            set => Preferences.Set(nameof(Theme), value);
        }
    }
}
