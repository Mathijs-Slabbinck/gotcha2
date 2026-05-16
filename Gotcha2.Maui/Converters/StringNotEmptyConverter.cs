using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    /* Maps a string to a bool — true when non-null and non-empty, false otherwise.
     * Used to bind IsVisible="{Binding SomeError, Converter={StaticResource StringNotEmptyConverter}}"
     * on per-field error Labels so they only render when their bound error string has content.
     *
     * One-way only: ConvertBack throws because mapping a bool back to a string has no meaningful inverse
     * (true → which string?). If someone wires this on a two-way binding by mistake, we want to know loudly. */
    public class StringNotEmptyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // string → bool
            if (value is string s && !string.IsNullOrEmpty(s))
                return true;

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException("StringNotEmptyConverter is one-way only.");
        }
    }
}
