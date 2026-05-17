using FluentValidation;
using Gotcha2.Maui.Models.Forms;

namespace Gotcha2.Maui.Validators
{
    public class SettingsValidator : AbstractValidator<SettingsData>
    {
        public SettingsValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Required");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Required");
        }
    }
}
