using Gotcha2.API.Dtos.Players.Response.Summary;

namespace Gotcha2.API.Dtos.TargetAssignments.Response
{
    // Used in: TargetAssignmentsController, MappingExtensions.
    // For: GET /api/games/{gameId}/my-target.
    public class TargetAssignmentResponseDto
    {
        public required Guid Id { get; init; }
        public required Guid GameId { get; init; }
        public required DateTime TargetAssigned { get; init; }
        public required PlayerSummaryDto Hunter { get; init; }
        public required PlayerSummaryDto Target { get; init; }
    }
}
