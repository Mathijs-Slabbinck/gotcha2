using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services
{
    public interface IAuthService
    {
        Task<ResultModel<AuthResponseDto>> SignInAsync(string email, string password);
        Task<ResultModel<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    }
}
