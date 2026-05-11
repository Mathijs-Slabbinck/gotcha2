using Gotcha2.Core.Enums;

namespace Gotcha2.API.Dtos.Users.Response
{
    // Used in: UsersController.
    // For: GET /api/users/me — own profile + computed stats.
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

        // Computed stats (not stored).
        public required int GamesPlayed { get; init; }
        public required int GamesWon { get; init; }
        public required int TotalKills { get; init; }
    }
}
