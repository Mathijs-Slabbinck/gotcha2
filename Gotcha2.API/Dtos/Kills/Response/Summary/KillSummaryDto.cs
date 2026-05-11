using Gotcha2.API.Dtos.Players.Response.Summary;

namespace Gotcha2.API.Dtos.Kills.Response.Summary
{
    // Used in: MappingExtensions, MatchResult / PlayerHome views.
    public class KillSummaryDto
    {
        public required Guid Id { get; init; }
        public required DateTime Moment { get; init; }
        public required PlayerSummaryDto Killer { get; init; }
        public required PlayerSummaryDto Victim { get; init; }
    }
}
