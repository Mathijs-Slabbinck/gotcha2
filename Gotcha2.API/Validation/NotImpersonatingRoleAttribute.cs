using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Validation
{
    // Used in: personal-name fields on RegisterRequestDto (FirstName, LastName).
    // Rejects values that match privileged role / system identifiers (admin, root, etc.) so
    // a regular user can't sign up as "Admin Admin" and then have that name surfaced anywhere
    // public-facing. Exact match on the trimmed lowercased value — substring matching would
    // false-positive on legitimate surnames like "Adminson" or "Rooth".
    // Null is treated as valid — [Required] owns the null path.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NotImpersonatingRoleAttribute : ValidationAttribute
    {
        // Stored lowercase — input is also lowercased before the lookup.
        // "user" is included even though it's our only role — a FirstName of "User" would be
        // confusing in the UI. "admin" stays as a guard against future re-introduction.
        private static readonly string[] ReservedRoles = new[]
        {
            "admin",
            "system",
            "root",
            "moderator",
            "void",
            "user"
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            if (value is not string text)
            {
                return new ValidationResult("Value must be a string.");
            }

            string trimmedLower = text.Trim().ToLower();

            if (ReservedRoles.Contains(trimmedLower))
            {
                return new ValidationResult($"{validationContext.DisplayName} cannot be a reserved role name.");
            }

            return ValidationResult.Success;
        }
    }
}
