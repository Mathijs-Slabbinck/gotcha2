using System.ComponentModel.DataAnnotations;
using Gotcha2.API.Dtos.Authentication.Base;
using Gotcha2.API.Validation;
using Gotcha2.Core.Enums;

namespace Gotcha2.API.Dtos.Authentication.Request
{
    // What the client sends to POST /api/auth/register.
    // [Required]/[EmailAddress]/etc. checked automatically by [ApiController] before the action runs.
    public class RegisterRequestDto : AuthBaseDto
    {
        [Required(ErrorMessage = "First name is required.")]
        [IsNotReservedString]
        [NotImpersonatingRole]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [IsNotReservedString]
        [NotImpersonatingRole]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birth date is required.")]
        [ValidBirthday(MinAge = 13, MaxAge = 180)]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(Genders), ErrorMessage = "Gender must be one of: Male, Female, Other.")]
        public Genders? Gender { get; set; }
    }
}
