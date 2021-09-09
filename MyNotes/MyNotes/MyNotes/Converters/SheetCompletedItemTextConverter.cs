using MyNotes.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyNotes.Converters
{
    public class SheetCompletedItemTextConverter : IValueConverter
    {
        public string Format { get; set; } = "{0}/{1}";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Sheet sheet)
            {
                var x = string.Format(Format, sheet.CompletedItemCount, sheet.ItemCount);
                return string.Format(Format, sheet.CompletedItemCount, sheet.ItemCount);
            }

            throw new ArgumentException("Value is not a valid Sheet", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
