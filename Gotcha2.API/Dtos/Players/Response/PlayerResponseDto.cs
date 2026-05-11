namespace Gotcha2.API.Dtos.Players.Response
{
    // Used in: PlayersController, MappingExtensions.
    // For: GET /api/players/{id}.
    public class PlayerResponseDto
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required bool HasProfileImage { get; init; }
        public required Guid GameId { get; init; }
        public required string GameName { get; init; }
        public required bool IsAlive { get; init; }
        public required string Notes { get; init; }
    }
}
