using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    /* Maps a string to a bool — true when the string has content, false when null/empty.
     * Used to bind IsVisible="{Binding ErrorMessage, Converter={StaticResource IsStringNotNullOrEmptyConverter}}"
     * so an error label only shows when there's actually an error to display.
     * One-way only: the reverse direction would have to invent a string from a bool, which is undefined. */
    public class IsStringNotNullOrEmptyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            string? text = value as string;
            return !string.IsNullOrEmpty(text);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("IsStringNotNullOrEmptyConverter is one-way.");
        }
    }
}
