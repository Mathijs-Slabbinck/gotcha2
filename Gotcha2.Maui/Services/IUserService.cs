using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services
{
    public interface IUserService
    {
        Task<ResultModel<UserItem>> GetMeAsync();
        Task<ResultModel<UserItem>> UpdateMeAsync(UserUpdateRequestDto request);
        Task<BaseResultModel> ChangePasswordAsync(ChangePasswordRequestDto request);
        Task<BaseResultModel> UpdateProfileImageAsync(byte[] bytes, string contentType, string fileName);
    }
}
