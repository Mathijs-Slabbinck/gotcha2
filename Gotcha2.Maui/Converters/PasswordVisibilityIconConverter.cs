using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    /* Maps a bool to an eye-icon filename — true → eye_open.svg, false → eye_closed.svg.
     * Used to bind Source="{Binding IsPasswordVisible, Converter={StaticResource PasswordVisibilityIconConverter}}"
     * on the show/hide ImageButton next to a password Entry.
     * One-way only: the reverse direction would require an arbitrary string→bool lookup table. */
    public class PasswordVisibilityIconConverter : IValueConverter
    {
        public const string eyeOpen = "eye_open.svg";
        public const string eyeClosed = "eye_closed.svg";

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // bool → string filename

            // If value is a bool & false
            if (value is bool isVisible && isVisible)
                return eyeOpen;

            return eyeClosed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // string filename → bool

            // If value is a string & "eye_open.svg"
            if (value is string s && s == eyeOpen)
                return true;

            return false;
        }
    }
}
