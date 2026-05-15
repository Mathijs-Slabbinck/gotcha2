namespace Gotcha2.Maui.Models.Items
{
    // UI-shaped row for the player-list CollectionView (MatchResult page).
    // Mapped from PlayerSummaryDto by ApiPlayerService — first/last name pre-flattened
    // so the XAML can bind directly.
    public class PlayerItem
    {
        public required Guid PlayerId { get; init; }
        public required Guid UserId { get; init; }
        public required string DisplayName { get; init; }
        public required bool IsAlive { get; init; }
        public required bool HasProfileImage { get; init; }
    }
}
