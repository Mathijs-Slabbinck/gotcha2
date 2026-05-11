using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    public interface IPlayerRepoService
    {
        Task<ResultModel<Player>> GetByIdAsync(Guid id);
        Task<ResultModel<Player>> DeleteAsync(Guid id);

        // All players in a given game, with User loaded so MapToPlayerSummaryDto can read FirstName/LastName.
        Task<ResultModel<List<Player>>> GetByGameAsync(Guid gameId);

        // Single-query membership check for authorization paths that don't need the player list.
        Task<ResultModel<bool>> IsUserInGameAsync(Guid gameId, Guid userId);

        // Number of games a user has joined (one Player row per game).
        Task<ResultModel<int>> CountByUserAsync(Guid userId);
    }
}
