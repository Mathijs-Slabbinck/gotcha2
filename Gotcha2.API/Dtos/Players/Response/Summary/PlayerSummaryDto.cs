namespace Gotcha2.API.Dtos.Players.Response.Summary
{
    // Used in: GameResponseDto, KillResponseDto, TargetAssignmentResponseDto, MappingExtensions.
    // Lightweight shape for embedded player references — no game info, no target list.
    public class PlayerSummaryDto
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required bool IsAlive { get; init; }
        public required bool HasProfileImage { get; init; }
    }
}
