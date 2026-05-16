using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    /* Maps a bool to an eye-icon filename — true → eye_open.svg, false → eye_closed.svg.
     * Used to bind Source="{Binding IsPasswordVisible, Converter={StaticResource PasswordVisibilityIconConverter}}"
     * on the show/hide ImageButton next to a password Entry.
     * One-way only: the reverse direction would require an arbitrary string→bool lookup table. */
    public class PasswordVisibilityIconConverter : IValueConverter
    {
        public const string EyeOpen = "eye_open.svg";
        public const string EyeClosed = "eye_closed.svg";

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isVisible && isVisible)
                return EyeOpen;

            return EyeClosed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (!(value is bool isVisible && isVisible))
                return EyeClosed;

            return EyeOpen;
        }
    }
}
