using Gotcha2.API.Dtos.Players.Response.Summary;

namespace Gotcha2.API.Dtos.Games.Response
{
    // Used in: GamesController (GET by id, POST, PUT, join, start), MappingExtensions.
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
        public List<PlayerSummaryDto> Players { get; init; } = new List<PlayerSummaryDto>();
        public required int KillCount { get; init; }
    }
}
