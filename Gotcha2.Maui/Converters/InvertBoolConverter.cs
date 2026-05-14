using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    /* Flips a bool. Used to bind IsEnabled="{Binding IsBusy, Converter={StaticResource InvertBool}}"
     * so form controls disable themselves while a command is running.
     * Register as a StaticResource in Resources/Styles/Converters.xaml (or App.xaml ResourceDictionary). */
    public class InvertBoolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }

            return false;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                return !b;
            }

            return false;
        }
    }
}
