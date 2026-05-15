namespace Gotcha2.Maui.Models.Dtos.Response
{
    public class AuthResponseDto
    {
        public required Guid UserId { get; init; }
        public required string Token { get; init; }
        public required DateTime ExpiresAtUtc { get; init; }
    }
}
