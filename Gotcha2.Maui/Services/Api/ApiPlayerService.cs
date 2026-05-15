using System.Net.Http.Json;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services.Api
{
    public class ApiPlayerService : IPlayerService
    {
        private readonly HttpClient _httpClient;

        public ApiPlayerService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GotchaApi");
        }

        public async Task<ResultModel<List<PlayerItem>>> GetByGameAsync(Guid gameId)
        {
            ResultModel<List<PlayerItem>> result = new ResultModel<List<PlayerItem>>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/games/{gameId}/players");

                if (response.IsSuccessStatusCode)
                {
                    List<PlayerSummaryDto>? data = await response.Content.ReadFromJsonAsync<List<PlayerSummaryDto>>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    List<PlayerItem> items = new List<PlayerItem>();

                    foreach (PlayerSummaryDto dto in data)
                    {
                        items.Add(ToPlayerItem(dto));
                    }

                    result.Data = items;
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
                System.Diagnostics.Debug.WriteLine($"ApiPlayerService.GetByGameAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        private static PlayerItem ToPlayerItem(PlayerSummaryDto dto)
        {
            return new PlayerItem
            {
                PlayerId = dto.Id,
                UserId = dto.UserId,
                DisplayName = $"{dto.FirstName} {dto.LastName}",
                IsAlive = dto.IsAlive,
                HasProfileImage = dto.HasProfileImage
            };
        }
    }
}
