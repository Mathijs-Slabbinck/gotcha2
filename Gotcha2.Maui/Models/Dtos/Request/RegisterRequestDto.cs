using Gotcha2.Maui.Enums;

namespace Gotcha2.Maui.Models.Dtos.Request
{
    public class RegisterRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required DateTime BirthDate { get; init; }
        public required Genders Gender { get; init; }
    }
}
