using System.Globalization;

namespace Gotcha2.Maui.Converters
{
    /* Maps a bool to an eye-icon filename — true → eye_open.svg, false → eye_closed.svg.
     * Used to bind Source="{Binding IsPasswordVisible, Converter={StaticResource PasswordVisibilityIconConverter}}"
     * on the show/hide ImageButton next to a password Entry.
     * Two-way safe: the forward range is the closed set {EyeOpen, EyeClosed}, so the inverse is well-defined
     * (string equals EyeOpen ⇒ true, anything else ⇒ false). */
    public class PasswordVisibilityIconConverter : IValueConverter
    {
        public const string EyeOpen = "eye_open.svg";
        public const string EyeClosed = "eye_closed.svg";

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // bool → string filename

            // If value is a bool & true
            if (value is bool isVisible && isVisible)
                return EyeOpen;

            return EyeClosed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // string filename → bool

            // If value is a string & "eye_open.svg"
            if (value is string s && s == EyeOpen)
                return true;

            return false;
        }
    }
}
