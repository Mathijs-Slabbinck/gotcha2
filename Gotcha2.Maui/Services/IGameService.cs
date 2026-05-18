using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services
{
    public interface IGameService
    {
        Task<ResultModel<List<GameItem>>> GetAllAsync();
        Task<ResultModel<GameDetailItem>> GetByIdAsync(Guid gameId);
        Task<ResultModel<GameItem>> CreateAsync(GameRequestDto request);
        Task<BaseResultModel> JoinAsync(Guid gameId);
        Task<BaseResultModel> StartAsync(Guid gameId);
        Task<BaseResultModel> LeaveAsync(Guid playerId);
        Task<ResultModel<List<KillItem>>> GetKillsAsync(Guid gameId);
        Task<ResultModel<KillItem>> RecordKillAsync(Guid gameId, KillRequestDto request);
        // There's a brief between-assignments window after a kill is recorded but before the server re-assigns,
        // so TargetAssignmentItem is nullable to allow for that possibility
        Task<ResultModel<TargetAssignmentItem?>> GetMyTargetAsync(Guid gameId);
    }
}
