using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    public interface IKillRepoService : IRepositoryService<Kill>
    {
        Task<ResultModel<List<Kill>>> GetByGameAsync(Guid gameId);

        // Records a kill, closes the open hunter->victim assignment, marks victim dead,
        // re-targets hunter at victim's old target, ends the game if only 1 alive remains.
        // `actingUserId` is the caller — must be either the assigned hunter OR the victim themselves.
        Task<ResultModel<Kill>> RecordKillAsync(Guid gameId, Guid victimPlayerId, Guid actingUserId);
    }
}
