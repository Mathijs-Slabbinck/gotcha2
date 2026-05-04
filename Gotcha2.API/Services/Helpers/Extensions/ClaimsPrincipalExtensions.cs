using System.Security.Claims;

namespace Gotcha2.API.Services.Helpers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            string? raw = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (raw is null)
            {
                throw new InvalidOperationException("NameIdentifier claim is missing. Ensure [Authorize] is applied.");
            }

            return Guid.Parse(raw);
        }
    }
}
