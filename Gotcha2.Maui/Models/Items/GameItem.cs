namespace Gotcha2.Maui.Models.Items
{
    // UI-shaped row for the Games-list CollectionView.
    // Mapped from GameSummaryDto by ApiGameService — dates pre-formatted
    public class GameItem
    {
        public required Guid GameId { get; init; }
        public required string Name { get; init; }
        public required string CreationDateText { get; init; }
        public required string StartDateText { get; init; }
        public required string EndDateText { get; init; }
        public required bool HasStarted { get; init; }
        public required bool IsFinished { get; init; }
        public required int PlayerCount { get; init; }
        public required int KillCount { get; init; }
        public required bool IsCreator { get; init; }
        public required Guid? CurrentUserPlayerId { get; init; }
        public required bool IsWinner { get; init; }
    }
}
