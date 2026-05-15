using System.Net.Http.Json;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services.Api
{
    public class ApiUserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public ApiUserService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GotchaApi");
        }

        public async Task<ResultModel<UserItem>> GetMeAsync()
        {
            ResultModel<UserItem> result = new ResultModel<UserItem>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/users/me");

                if (response.IsSuccessStatusCode)
                {
                    UserResponseDto? data = await response.Content.ReadFromJsonAsync<UserResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    result.Data = ToUserItem(data);
                    return result;
                }

                List<string>? errors = await response.Content.ReadFromJsonAsync<List<string>>();

                if (errors is not null && errors.Count > 0)
                {
                    foreach (string err in errors)
                    {
                        result.Errors.Add(err);
                    }
                }
                else
                {
                    result.Errors.Add("Something went wrong. Please try again.");
                }

                return result;
            }
            catch (Exception ex)
            {
                // Normally I would use a logging framework,
                // but a simple Debug.WriteLine is sufficient for this example and easier to explain
                System.Diagnostics.Debug.WriteLine($"ApiUserService.GetMeAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<UserItem>> UpdateMeAsync(UserUpdateRequestDto request)
        {
            ResultModel<UserItem> result = new ResultModel<UserItem>();

            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("api/users/me", request);

                if (response.IsSuccessStatusCode)
                {
                    UserResponseDto? data = await response.Content.ReadFromJsonAsync<UserResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    result.Data = ToUserItem(data);
                    return result;
                }

                List<string>? errors = await response.Content.ReadFromJsonAsync<List<string>>();

                if (errors is not null && errors.Count > 0)
                {
                    foreach (string err in errors)
                    {
                        result.Errors.Add(err);
                    }
                }
                else
                {
                    result.Errors.Add("Something went wrong. Please try again.");
                }

                return result;
            }
            catch (Exception ex)
            {
                // Normally I would use a logging framework,
                // but a simple Debug.WriteLine is sufficient for this example and easier to explain
                System.Diagnostics.Debug.WriteLine($"ApiUserService.UpdateMeAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<BaseResultModel> ChangePasswordAsync(ChangePasswordRequestDto request)
        {
            BaseResultModel result = new BaseResultModel();

            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("api/users/me/password", request);

                if (response.IsSuccessStatusCode)
                    return result;

                List<string>? errors = await response.Content.ReadFromJsonAsync<List<string>>();

                if (errors is not null && errors.Count > 0)
                {
                    foreach (string err in errors)
                    {
                        result.Errors.Add(err);
                    }
                }
                else
                {
                    result.Errors.Add("Something went wrong. Please try again.");
                }

                return result;
            }
            catch (Exception ex)
            {
                // Normally I would use a logging framework,
                // but a simple Debug.WriteLine is sufficient for this example and easier to explain
                System.Diagnostics.Debug.WriteLine($"ApiUserService.ChangePasswordAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        private static UserItem ToUserItem(UserResponseDto dto)
        {
            return new UserItem
            {
                UserId = dto.Id,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DisplayName = $"{dto.FirstName} {dto.LastName}",
                Gender = dto.Gender,
                BirthDate = dto.BirthDate,
                AccountCreationDateText = dto.AccountCreationDate.ToString("dd MMM yyyy"),
                HasProfileImage = dto.HasProfileImage,
                GamesPlayed = dto.GamesPlayed,
                GamesWon = dto.GamesWon,
                TotalKills = dto.TotalKills
            };
        }
    }
}
