using FluentValidation;
using Gotcha2.Maui.Models.Forms;

namespace Gotcha2.Maui.Validators
{
    public class SignUpValidator : AbstractValidator<SignUpData>
    {
        public SignUpValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Required");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Required");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Required")
                .MinimumLength(8).WithMessage("Must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Must contain an uppercase letter")
                .Matches("[a-z]").WithMessage("Must contain a lowercase letter")
                .Matches("[0-9]").WithMessage("Must contain a digit")
                .Matches("[^a-zA-Z0-9]").WithMessage("Must contain a symbol");

            RuleFor(x => x.ConfirmPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Required")
                .Equal(x => x.Password).WithMessage("Passwords don't match");

            RuleFor(x => x.BirthDate)
                .Cascade(CascadeMode.Stop)
                .Must(BeNotInFuture).WithMessage("Birthday can't be in the future")
                .Must(BeAtLeast13YearsOld).WithMessage("You must be at least 13 years old")
                .Must(BeAtMost180YearsOld).WithMessage("Birthday isn't realistic");

            RuleFor(x => x.Gender)
                .NotNull().WithMessage("Required");
        }


        /* --- comment ---
         * I chose 3 separate Must rules on BirthDate (BeNotInFuture → BeAtLeast13YearsOld → BeAtMost180YearsOld)
         * rather than one combined predicate, so each gets its own message.
         * Cascade.Stop ensures only the first failing one fires. */
        private static bool BeNotInFuture(DateTime birthDate)
        {
            return birthDate.Date <= DateTime.Today;
        }

        private static bool BeAtLeast13YearsOld(DateTime birthDate)
        {
            return CalculateAge(birthDate) >= 13;
        }

        private static bool BeAtMost180YearsOld(DateTime birthDate)
        {
            return CalculateAge(birthDate) <= 180;
        }

        private static int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;

            // If the birthday hasn't occurred yet this year, subtract one.
            if (birthDate.Date > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
