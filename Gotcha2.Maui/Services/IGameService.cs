using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services
{
    public interface IGameService
    {
        Task<ResultModel<List<GameItem>>> GetAllAsync();
        Task<ResultModel<GameItem>> CreateAsync(GameRequestDto request);
        Task<BaseResultModel> JoinAsync(Guid gameId);
    }
}
