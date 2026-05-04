using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Dtos.Authentication.Base
{
    // Used in: LoginRequestDto, RegisterRequestDto.
    // Shared Email + Password fields. Password rules themselves (length, digits, symbols)
    // are enforced by Identity's PasswordValidator (configured in Program.cs).
    public abstract class AuthBaseDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
