namespace Gotcha2.Maui.Models.Items
{
    // UI-shaped projection for a single fully-detailed game (PlayerHome page).
    // Mapped from GameResponseDto by ApiGameService — dates pre-formatted, embedded Players list
    // mapped to PlayerItem so the XAML can bind directly.
    public class GameDetailItem
    {
        public required Guid GameId { get; init; }
        public required string Name { get; init; }
        public required string CreationDateText { get; init; }
        public required string StartDateText { get; init; }
        public required string EndDateText { get; init; }
        public required bool HasStarted { get; init; }
        public required bool IsFinished { get; init; }
        public required int KillCount { get; init; }
        public required bool IsCreator { get; init; }
        public required Guid CreatorId { get; init; }
        public required Guid? WinnerId { get; init; }
        public required Guid? CurrentUserPlayerId { get; init; }
        public required bool IsWinner { get; init; }
        public required IList<PlayerItem> Players { get; init; }

        public bool IsPending => !HasStarted && !IsFinished;
        public bool IsActive => HasStarted && !IsFinished;
        public bool IsPast => IsFinished;
    }
}
