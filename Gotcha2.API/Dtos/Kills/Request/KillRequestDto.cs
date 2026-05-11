using System.ComponentModel.DataAnnotations;

namespace Gotcha2.API.Dtos.Kills.Request
{
    // Used in: KillsController.
    // For: POST /api/games/{gameId}/kills.
    // The caller is identified via JWT; the body only needs to identify the victim.
    public class KillRequestDto
    {
        [Required(ErrorMessage = "VictimPlayerId is required.")]
        public Guid? VictimPlayerId { get; set; }
    }
}
