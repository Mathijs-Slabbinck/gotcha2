namespace Gotcha2.API.Dtos.Games.Response.Summary
{
    // Used in: GamesController (GET all by user — list view), MappingExtensions.
    // Compact shape for the Games-list page on MAUI; no players or kills embedded.
    public class GameSummaryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required DateTime CreationDate { get; init; }
        public required DateTime? StartDate { get; init; }
        public required DateTime? EndDate { get; init; }
        public required bool HasStarted { get; init; }
        public required bool IsFinished { get; init; }
        public required Guid? WinnerId { get; init; }
        public required Guid CreatorId { get; init; }
        public required int PlayerCount { get; init; }
    }
}
