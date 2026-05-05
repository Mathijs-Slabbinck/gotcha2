using Gotcha2.API.Dtos.Players.Response.Summary;

namespace Gotcha2.API.Dtos.Kills.Response
{
    // Used in: KillsController, MappingExtensions.
    public class KillResponseDto
    {
        public required Guid Id { get; init; }
        public required Guid GameId { get; init; }
        public required DateTime Moment { get; init; }
        public required PlayerSummaryDto Killer { get; init; }
        public required PlayerSummaryDto Victim { get; init; }
    }
}
