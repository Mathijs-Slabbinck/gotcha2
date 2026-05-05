using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    // Bare CRUD inherited from IRepositoryService<Player>.
    // Domain-specific reads added here.
    public interface IPlayerRepoService : IRepositoryService<Player>
    {
        // All players in a given game, with User loaded so MapToPlayerSummaryDto can read FirstName/LastName.
        Task<ResultModel<List<Player>>> GetByGameAsync(Guid gameId);
    }
}
