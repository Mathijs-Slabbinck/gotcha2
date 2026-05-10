using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    public interface IGameRepoService
    {
        Task<ResultModel<Game>> GetByIdAsync(Guid id);
        Task<ResultModel<Game>> AddAsync(Game game);
        Task<ResultModel<Game>> UpdateAsync(Game game);
        Task<ResultModel<Game>> DeleteAsync(Guid id);

        // Games where the given user has a Player row.
        Task<ResultModel<List<Game>>> GetByUserAsync(Guid userId);

        // Number of finished games where the user is the recorded winner.
        Task<ResultModel<int>> CountWinsByUserAsync(Guid userId);

        // User joins as a Player. Validates: game exists, not started, not already joined.
        Task<ResultModel<Game>> JoinAsync(Guid gameId, Guid userId);

        // Creator starts the game. Validates: caller is creator, >= 2 players, not already started.
        // Builds the target-assignment cycle (every player has exactly one open assignment).
        Task<ResultModel<Game>> StartAsync(Guid gameId, Guid creatorUserId);
    }
}
