namespace Gotcha2.Maui.Models.Items
{
    // UI-shaped row for the kill-list CollectionView (MatchResult / PlayerHome pages).
    // Mapped from KillSummaryDto or KillResponseDto by ApiGameService — moment pre-formatted,
    // killer/victim names pre-flattened so the XAML can bind directly.
    public class KillItem
    {
        public required Guid KillId { get; init; }
        public required Guid GameId { get; init; }
        public required string MomentText { get; init; }
        public required string KillerDisplayName { get; init; }
        public required string VictimDisplayName { get; init; }
    }
}
