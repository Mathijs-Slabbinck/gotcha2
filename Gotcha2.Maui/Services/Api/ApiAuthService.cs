using System.Net.Http.Json;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services.Api
{
    public class ApiAuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public ApiAuthService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GotchaApi");
        }

        public async Task<ResultModel<AuthResponseDto>> SignInAsync(string email, string password)
        {
            ResultModel<AuthResponseDto> result = new ResultModel<AuthResponseDto>();

            try
            {
                LoginRequestDto payload = new LoginRequestDto { Email = email, Password = password };
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/login", payload);

                if (response.IsSuccessStatusCode)
                {
                    AuthResponseDto? data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    result.Data = data;
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
                // but a siple Debug.WriteLine is sufficient for this example and easier to explain
                System.Diagnostics.Debug.WriteLine($"ApiAuthService.SignInAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
        {
            ResultModel<AuthResponseDto> result = new ResultModel<AuthResponseDto>();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

                if (response.IsSuccessStatusCode)
                {
                    AuthResponseDto? data = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    result.Data = data;
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
                System.Diagnostics.Debug.WriteLine($"ApiAuthService.RegisterAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }

            /* --- comment ---
             * Logout lives at SessionService,
             * This is because logout is a purely client-side operation
             * (clearing the token and user id from memory and secure storage) */
        }
    }
}
