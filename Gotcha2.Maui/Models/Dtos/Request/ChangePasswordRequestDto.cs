namespace Gotcha2.Maui.Models.Dtos.Request
{
    public class ChangePasswordRequestDto
    {
        public required string NewPassword { get; init; }
        public required string ConfirmNewPassword { get; init; }
    }
}
