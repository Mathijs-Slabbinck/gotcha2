namespace Gotcha2.Maui.Models.Dtos.Response
{
    // Wire shape returned by GET /api/games/{gameId}/my-target.
    // Mirrors Gotcha2.API/Dtos/TargetAssignments/Response/TargetAssignmentResponseDto.cs.
    public class TargetAssignmentResponseDto
    {
        public required Guid Id { get; init; }
        public required Guid GameId { get; init; }
        public required DateTime TargetAssigned { get; init; }
        public required PlayerSummaryDto Hunter { get; init; }
        public required PlayerSummaryDto Target { get; init; }
    }
}
