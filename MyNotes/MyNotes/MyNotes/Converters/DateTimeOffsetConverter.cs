using System;
using System.Globalization;
using Xamarin.Forms;

namespace MyNotes.Converters
{
    public class DateTimeOffsetConverter : IValueConverter
    {
        public string Format { get; set; } = "dd/MM/yy HH:mm";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                dateTimeOffset = dateTimeOffset.ToLocalTime();
                return dateTimeOffset.ToString(Format, CultureInfo.InvariantCulture);
            }

            throw new ArgumentException("Value is not a valid DateTimeOffset", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
