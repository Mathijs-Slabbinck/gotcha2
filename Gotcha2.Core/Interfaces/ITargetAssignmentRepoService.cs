using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Services.Models;

namespace Gotcha2.Core.Interfaces
{
    public interface ITargetAssignmentRepoService : IRepositoryService<TargetAssignment>
    {
        // Currently-open assignment where the caller is the Hunter, in the given game.
        // Returns null Data with no errors if the caller has no open assignment (dead, or game ended).
        Task<ResultModel<TargetAssignment?>> GetMyTargetAsync(Guid gameId, Guid userId);
    }
}
