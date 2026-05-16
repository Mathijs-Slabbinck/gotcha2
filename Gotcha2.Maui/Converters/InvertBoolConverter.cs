using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    // Maui comes with a build in bool converter, but we decided to write our own since it's a small file to maintain
    // and since we need other converters anyways, it's good to stay consistent and use our own converters for all our needs.

    /* Flips a bool. Used to bind IsEnabled="{Binding IsBusy, Converter={StaticResource InvertBool}}"
     * so form controls disable themselves while a command is running. */
    public class InvertBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;

            return false;
        }
    }
}
