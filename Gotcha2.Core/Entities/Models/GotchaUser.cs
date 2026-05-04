using Gotcha2.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Gotcha2.Core.Entities.Models
{
    public class GotchaUser : IdentityUser<Guid>
    {
        public override Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Inherited from IdentityUser (GotchaUser.Email)
        public required override string Email { get; set; }
        public bool HasProfileImage { get; set; }
        public Genders Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime AccountCreationDate { get; init; } = DateTime.UtcNow;
        public ICollection<Player> PlayerAccounts { get; set; } = new List<Player>();
        public bool IsDeleted { get; set; }

        public override string ToString()
        {
            return $"({FirstName} {LastName})";
        }
    }
}
