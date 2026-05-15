namespace Gotcha2.Maui.Models.Dtos.Response
{
    /* Returned by POST /api/games/{gameId}/kills.
     * Carries GameId on the wire (unlike KillSummaryDto) since the create response is not scoped by a list-endpoint route.
     * ApiGameService maps it to KillItem. */
    public class KillResponseDto
    {
        public required Guid Id { get; init; }
        public required Guid GameId { get; init; }
        public required DateTime Moment { get; init; }
        public required PlayerSummaryDto Killer { get; init; }
        public required PlayerSummaryDto Victim { get; init; }
    }
}
