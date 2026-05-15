namespace Gotcha2.Maui.Models.Dtos.Response
{
    // Wire mirror of Gotcha2.API/Dtos/Games/Response/GameResponseDto.cs.
    // Returned by POST /api/games, GET /api/games/{id}, and (currently unused MAUI-side)
    // join/start/update endpoints. ApiGameService maps it to GameItem for UI consumption.
    public class GameResponseDto
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
        public required int KillCount { get; init; }
        public List<PlayerSummaryDto> Players { get; init; } = new List<PlayerSummaryDto>();
    }
}
