using Gotcha2.Core.Constants;
using Microsoft.AspNetCore.Identity;

namespace Gotcha2.Core.Data.Seeder
{
    // Idempotent startup seeder. Phase 1 only seeds the User role so that
    // AuthController.Register's AddToRoleAsync(user, Roles.User) doesn't throw
    // on the very first registration. Phase 3 will extend this class with SeedDataAsync
    // for test users + a sample game.
    public static class Seeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
        {
            bool exists = await roleManager.RoleExistsAsync(Roles.User);

            if (!exists)
            {
                IdentityRole<Guid> role = new IdentityRole<Guid>(Roles.User);
                IdentityResult result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed role '{Roles.User}': {errors}");
                }
            }
        }
    }
}
