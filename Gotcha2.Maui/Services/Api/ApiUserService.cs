using System.Net.Http.Headers;
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

        public async Task<BaseResultModel> UpdateProfileImageAsync(byte[] bytes, string contentType, string fileName)
        {
            BaseResultModel result = new BaseResultModel();

            try
            {
                // The API endpoint is [FromForm] IFormFile file - the multipart form field name must be exactly "file".

                // Create an empty container that will become the request body.
                MultipartFormDataContent content = new MultipartFormDataContent();
                // Wrap the photo's raw bytes into something HttpContent-compatible.
                ByteArrayContent fileContent = new ByteArrayContent(bytes);
                // Stamp the part's Content-Type header with whatever the picked photo's MIME type was (image/jpeg, image/png, etc.)
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                // Add the file content to the multipart form data container,
                // specifying the field name ("file") and the original file name
                // (we don't need this value, but we do need the param for the MultipartFormDataContent overload)
                content.Add(fileContent, "file", fileName);

                HttpResponseMessage response = await _httpClient.PutAsync("api/users/me/profile-image", content);

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
                System.Diagnostics.Debug.WriteLine($"ApiUserService.UpdateProfileImageAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<byte[]>> GetProfileImageAsync(Guid userId)
        {
            ResultModel<byte[]> result = new ResultModel<byte[]>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/users/{userId}/profile-image");

                if (response.IsSuccessStatusCode)
                {
                    byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                    result.Data = bytes;
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
                System.Diagnostics.Debug.WriteLine($"ApiUserService.GetProfileImageAsync failed: {ex.Message}");

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
