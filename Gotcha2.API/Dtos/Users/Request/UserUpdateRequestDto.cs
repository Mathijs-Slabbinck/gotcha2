using System.ComponentModel.DataAnnotations;
using Gotcha2.API.Validation;

namespace Gotcha2.API.Dtos.Users.Request
{
    // Used in: UsersController.
    // For: PUT /api/users/me. BirthDate / Gender are intentionally not editable post-signup.
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
    }
}
