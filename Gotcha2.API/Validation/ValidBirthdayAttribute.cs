using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Validation
{
    // Used in: RegisterRequestDto.BirthDate
    // Checks the value is a real-looking birthday: at least MinAge years old,
    // and not older than MaxAge years (rejects obvious junk like 1800-01-01).
    // Also rejects future dates.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ValidBirthdayAttribute : ValidationAttribute
    {
        public int MinAge { get; set; } = 13;
        public int MaxAge { get; set; } = 120;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // [Required] handles null — if we got here with null, treat as valid (skip).
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not DateTime dateOfBirth)
            {
                return new ValidationResult("Date of birth must be a valid date.");
            }

            DateTime today = DateTime.UtcNow.Date;

            if (dateOfBirth.Date > today)
            {
                return new ValidationResult("Date of birth cannot be in the future.");
            }

            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }

            if (age < MinAge)
            {
                return new ValidationResult($"You must be at least {MinAge} years old.");
            }

            if (age > MaxAge)
            {
                return new ValidationResult($"Date of birth is not realistic (age over {MaxAge}).");
            }

            return ValidationResult.Success;
        }
    }
}
