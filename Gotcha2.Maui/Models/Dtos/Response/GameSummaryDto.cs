namespace Gotcha2.Maui.Models.Dtos.Response
{
    // Consumed by ApiGameService.GetAllAsync and mapped to GameItem for the UI.
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
        public required int KillCount { get; init; }
        public required Guid? CurrentUserPlayerId { get; init; }
    }
}
