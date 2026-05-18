namespace Gotcha2.Maui.Models.Items
{
    // UI-shaped projection of the caller's currently-open target assignment (PlayerHome Active state).
    // Mapped from TargetAssignmentResponseDto by ApiGameService — date pre-formatted,
    // target name pre-flattened so the XAML can bind directly.
    public class TargetAssignmentItem
    {
        public required Guid AssignmentId { get; init; }
        public required Guid GameId { get; init; }
        public required string AssignedDateText { get; init; }
        public required Guid TargetPlayerId { get; init; }
        public required Guid TargetUserId { get; init; }
        public required string TargetDisplayName { get; init; }
        public required bool TargetHasProfileImage { get; init; }
    }
}
