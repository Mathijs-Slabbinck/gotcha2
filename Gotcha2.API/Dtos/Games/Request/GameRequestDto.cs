using System.ComponentModel.DataAnnotations;
using Gotcha2.API.Validation;

namespace Gotcha2.API.Dtos.Games.Request
{
    // Used in: GamesController.
    // For: POST /api/games.
    public class GameRequestDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        [IsNotReservedString]
        public string Name { get; set; } = string.Empty;
    }
}
