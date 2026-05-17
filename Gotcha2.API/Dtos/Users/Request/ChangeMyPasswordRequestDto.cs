using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Dtos.Users.Request
{
    // Used in: UsersController.
    // For: PUT /api/users/me/change-password. Authenticated change-password flow — caller must prove
    // they know the current password. Separate from ChangePasswordRequestDto, which is the forgot-password
    // surface (no current-password proof) and is intentionally kept narrow.
    public class ChangeMyPasswordRequestDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password confirmation is required.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
