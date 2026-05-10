using Gotcha2.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Gotcha2.Core.Entities.Models
{
    public class GotchaUser : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public bool HasProfileImage { get; set; } // = false
        public Genders Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime AccountCreationDate { get; init; } = DateTime.UtcNow;
        public ICollection<Player> PlayerAccounts { get; set; } = new List<Player>();
        public bool IsDeleted { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
