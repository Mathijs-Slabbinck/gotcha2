using System.ComponentModel.DataAnnotations;
using Gotcha2.API.Validation;
using Gotcha2.Core.Enums;

namespace Gotcha2.API.Dtos.Users.Request
{
    // Used in: UsersController.
    // For: PUT /api/users/me. Editable profile fields: Email (login key), FirstName/LastName, Gender.
    // BirthDate is locked after signup (not on this DTO — server preserves the original value on update).
    public class UserUpdateRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required.")]
        [IsNotReservedString]
        [NotImpersonatingRole]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [IsNotReservedString]
        [NotImpersonatingRole]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [EnumDataType(typeof(Genders), ErrorMessage = "Gender must be one of: Male, Female, Other.")]
        public Genders? Gender { get; set; }
    }
}
