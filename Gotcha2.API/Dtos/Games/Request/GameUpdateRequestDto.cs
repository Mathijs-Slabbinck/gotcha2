using System.ComponentModel.DataAnnotations;
using Gotcha2.API.Validation;

namespace Gotcha2.API.Dtos.Games.Request
{
    // Used in: GamesController.
    // For: PUT /api/games (no id in URL — id is in the body, matches reference convention).
    // Only the name is mutable post-create.
    public class GameUpdateRequestDto
    {
        [Required(ErrorMessage = "Id is required.")]
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name must be 100 characters or fewer.")]
        [IsNotReservedString]
        public string Name { get; set; } = string.Empty;
    }
}
