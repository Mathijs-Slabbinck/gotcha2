using Gotcha2.Maui.Enums;

namespace Gotcha2.Maui.Models.Forms
{
    public class SignUpData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.Today;
        // Picker.SelectedItem supports a null state, so we use a nullable enum here to allow "no selection" as the default
        // We set it as required in SinupValidator
        public Genders? Gender { get; set; }
    }
}
