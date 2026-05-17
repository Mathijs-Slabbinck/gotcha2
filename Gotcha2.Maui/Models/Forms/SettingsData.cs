using Gotcha2.Maui.Enums;

namespace Gotcha2.Maui.Models.Forms
{
    public class SettingsData
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Picker.SelectedItem supports a null state, so we use a nullable enum here to allow "no selection".
        // SettingsValidator marks it as required.
        public Genders? Gender { get; set; }
    }
}
