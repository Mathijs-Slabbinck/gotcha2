namespace Gotcha2.Maui.Models.Dtos.Request
{
    public class LoginRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
