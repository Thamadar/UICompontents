using Avalonia.Data.Converters;
using System; 
using System.Globalization; 

namespace Lib.Avalonia.Converters
{
    /// <summary>
    /// Конвертер, с помощью которого дата пишется с заглавных букв.
    /// "понедельник, март 26" => Понедельник, Март 26
    /// </summary>
    public class CapitalizeDateConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is not DateTime date)
                return string.Empty;

            var text = date.ToString("dddd, MMMM d", culture);
            return culture.TextInfo.ToTitleCase(text);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
