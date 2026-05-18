using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Validation
{
    // Used in: any string DTO field that ends up as user-facing content (names, titles).
    // Rejects a small fixed blocklist of "funny" inputs people paste when they're testing
    // a form (literal "null", "undefined", "[object Object]", "NaN") and rejects strings
    // that are only whitespace. Comparison is case-insensitive on the trimmed value.
    // Null is treated as valid — [Required] owns the null path.
    // [Required] / [StringLength] still do the heavy lifting — this is a thin extra check.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IsNotReservedStringAttribute : ValidationAttribute
    {
        private static readonly string[] ReservedValues = new[]
        {
            "null",
            "undefined",
            "[object object]",
            "nan"
        };

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return ValidationResult.Success;

            if (value is not string text)
                return new ValidationResult("Value must be a string.");

            string trimmed = text.Trim();

            if (trimmed.Length == 0)
                return new ValidationResult($"{validationContext.DisplayName} cannot be empty or whitespace.");

            if (ReservedValues.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                return new ValidationResult($"{validationContext.DisplayName} is not a valid value.");

            return ValidationResult.Success;
        }
    }
}
