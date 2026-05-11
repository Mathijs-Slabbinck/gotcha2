using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Dtos.Users.Request
{
    // Used in: UsersController.
    // For: PUT /api/users/me/password. The Identity PasswordValidator enforces full rules; this DTO
    // only short-circuits the obvious "missing field" / "too short" / "mismatch" cases for a friendlier 400.
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
