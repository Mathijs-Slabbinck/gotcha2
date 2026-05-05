using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Dtos.Users.Request
{
    // Used in: UsersController.
    // For: PUT /api/users/me/password. The Identity PasswordValidator enforces full rules; this DTO
    // only short-circuits the obvious "missing field" / "too short" cases for a friendlier 400.
    public class ChangePasswordRequestDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
