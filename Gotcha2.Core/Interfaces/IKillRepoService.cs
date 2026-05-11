using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    public interface IKillRepoService
    {
        Task<ResultModel<List<Kill>>> GetByGameAsync(Guid gameId);

        // Number of kills where the killer Player belongs to the given user.
        Task<ResultModel<int>> CountByKillerUserAsync(Guid userId);

        // Records a kill, closes the open hunter->victim assignment, marks victim dead,
        // re-targets hunter at victim's old target, ends the game if only 1 alive remains.
        // `actingUserId` is the caller — must be either the assigned hunter OR the victim themselves.
        Task<ResultModel<Kill>> RecordKillAsync(Guid gameId, Guid victimPlayerId, Guid actingUserId);
    }
}
