using FluentValidation;
using Gotcha2.Maui.Models.Forms;

namespace Gotcha2.Maui.Validators
{
    public class ChangeMyPasswordValidator : AbstractValidator<ChangeMyPasswordData>
    {
        public ChangeMyPasswordValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Required");

            // Same password rules as SignUpValidator — mirror Identity's PasswordValidator on the server.
            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Required")
                .MinimumLength(8).WithMessage("Must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Must contain an uppercase letter")
                .Matches("[a-z]").WithMessage("Must contain a lowercase letter")
                .Matches("[0-9]").WithMessage("Must contain a digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain a symbol");

            RuleFor(x => x.ConfirmNewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Required")
                .Equal(x => x.NewPassword).WithMessage("Passwords don't match");
        }
    }
}
