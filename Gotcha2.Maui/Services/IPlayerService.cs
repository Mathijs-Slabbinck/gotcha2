using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services
{
    public interface IPlayerService
    {
        Task<ResultModel<List<PlayerItem>>> GetByGameAsync(Guid gameId);
    }
}
