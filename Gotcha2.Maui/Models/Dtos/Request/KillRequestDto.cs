namespace Gotcha2.Maui.Models.Dtos.Request
{
    /* Body for POST /api/games/{gameId}/kills.
     * Caller (hunter or victim) is identified server-side via the JWT;
     * the body only needs to identify the victim. */
    public class KillRequestDto
    {
        public required Guid VictimPlayerId { get; init; }
    }
}
