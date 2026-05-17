namespace Gotcha2.Maui.Models.Dtos.Request
{
    public class ChangeMyPasswordRequestDto
    {
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
        public required string ConfirmNewPassword { get; init; }
    }
}
