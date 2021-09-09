using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyNotes.Converters
{
    class ItemStatusToTextDecorationConverter : IValueConverter
    {
        private TextDecorations defaultTextDecor = TextDecorations.None;
        private TextDecorations doneStatusTextDecor = TextDecorations.Strikethrough;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDone)
            {
                return isDone ? doneStatusTextDecor : defaultTextDecor;
            }

            return defaultTextDecor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return defaultTextDecor;
        }
    }
}
