namespace Gotcha2.Maui.Models.Dtos.Response
{
    /* Returned by GET /api/games/{gameId}/kills.
     * Mapped to KillItem for the UI.
     * No GameId on the wire - the route already scopes it;
     * ApiGameService passes gameId into the mapper from the request parameter. */
    public class KillSummaryDto
    {
        public required Guid Id { get; init; }
        public required DateTime Moment { get; init; }
        public required PlayerSummaryDto Killer { get; init; }
        public required PlayerSummaryDto Victim { get; init; }
    }
}
