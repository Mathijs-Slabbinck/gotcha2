using Gotcha2.Maui.Enums;

namespace Gotcha2.Maui.Models.Items
{
    /* UI-shaped row for user-profile display + Settings form pre-population.
     * Mapped from UserResponseDto by ApiUserService.
     * Home page formats BirthDate with XAML StringFormat — no BirthDateText to avoid duplication / drift. */
    public class UserItem
    {
        public required Guid UserId { get; init; }
        public required string Email { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string DisplayName { get; init; }
        public required Genders Gender { get; init; }
        public required DateTime BirthDate { get; init; }
        // pre-formatted text for the account creation date, since that's never edited and always shown in the same format.
        // Avoids duplication of formatting logic in XAML and code.
        public required string AccountCreationDateText { get; init; }
        public required bool HasProfileImage { get; init; }
        public required int GamesPlayed { get; init; }
        public required int GamesWon { get; init; }
        public required int TotalKills { get; init; }
    }
}
