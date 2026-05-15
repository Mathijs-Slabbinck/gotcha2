namespace Gotcha2.Maui.Models.Dtos.Response
{
    // Wire mirror of Gotcha2.API/Dtos/Players/Response/Summary/PlayerSummaryDto.cs.
    // Embedded inside GameResponseDto.Players, KillSummaryDto, KillResponseDto.
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
