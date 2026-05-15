using Gotcha2.Maui.Enums;

namespace Gotcha2.Maui.Models.Dtos.Response
{
    public class UserResponseDto
    {
        public required Guid Id { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required Genders Gender { get; init; }
        public required DateTime BirthDate { get; init; }
        public required DateTime AccountCreationDate { get; init; }
        public required bool HasProfileImage { get; init; }
        public required int GamesPlayed { get; init; }
        public required int GamesWon { get; init; }
        public required int TotalKills { get; init; }
    }
}
