using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    public interface IGameRepoService : IRepositoryService<Game>
    {
        // Games where the given user has a Player row.
        Task<ResultModel<List<Game>>> GetByUserAsync(Guid userId);

        // User joins as a Player. Validates: game exists, not started, not already joined.
        Task<ResultModel<Game>> JoinAsync(Guid gameId, Guid userId);

        // Creator starts the game. Validates: caller is creator, >= 2 players, not already started.
        // Builds the target-assignment cycle (every player has exactly one open assignment).
        Task<ResultModel<Game>> StartAsync(Guid gameId, Guid creatorUserId);
    }
}
