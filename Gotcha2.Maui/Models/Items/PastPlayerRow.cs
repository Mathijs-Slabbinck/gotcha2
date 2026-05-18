namespace Gotcha2.Maui.Models.Items
{
    // UI-shaped row for the all-players standings list on PlayerHome (Past state).
    // Built in PlayerHomeViewModel from GameDetailItem.Players + the kill list — Status is
    // pre-flattened to "Winner" / "Killed by {Name}" / "Eliminated" so the XAML can bind directly.
    public class PastPlayerRow
    {
        public required Guid PlayerId { get; init; }
        public required Guid UserId { get; init; }
        public required string DisplayName { get; init; }
        public required bool IsWinner { get; init; }
        public required string Status { get; init; }
        public required bool HasProfileImage { get; init; }
    }
}
