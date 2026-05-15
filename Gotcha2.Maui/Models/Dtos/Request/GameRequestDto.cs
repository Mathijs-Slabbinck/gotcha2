namespace Gotcha2.Maui.Models.Dtos.Request
{
    // Wire mirror of Gotcha2.API/Dtos/Games/Request/GameRequestDto.cs.
    // Body for POST /api/games. Immutable on purpose — forms bind to a separate writable
    // NewGameForm and construct this DTO at submit time.
    public class GameRequestDto
    {
        public required string Name { get; init; }
    }
}
