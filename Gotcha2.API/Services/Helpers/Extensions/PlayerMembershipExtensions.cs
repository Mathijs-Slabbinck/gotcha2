using Gotcha2.Core.Entities.Models;

namespace Gotcha2.API.Services.Helpers.Extensions
{
    public static class PlayerMembershipExtensions
    {
        public static bool IsMember(this Game game, Guid userId)
        {
            return game.Players.Any(p => p.UserId == userId);
        }

        public static bool IsMember(this IEnumerable<Player> players, Guid userId)
        {
            return players.Any(p => p.UserId == userId);
        }
    }
}
