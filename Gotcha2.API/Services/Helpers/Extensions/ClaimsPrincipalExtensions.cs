using System.Security.Claims;

namespace Gotcha2.API.Services.Helpers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Callers must guard with [Authorize], so the NameIdentifier claim is always present
        // and is a parseable Guid (we put it there ourselves in AuthController.GenerateTokenAsync).
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            string raw = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Guid.Parse(raw);
        }
    }
}
