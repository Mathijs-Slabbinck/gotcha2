using System.Net.Http.Json;
using Gotcha2.Maui.Models.Dtos.Request;
using Gotcha2.Maui.Models.Dtos.Response;
using Gotcha2.Maui.Models.Items;
using Gotcha2.Maui.Models.Result;

namespace Gotcha2.Maui.Services.Api
{
    public class ApiGameService : IGameService
    {
        private readonly HttpClient _httpClient;
        private readonly SessionService _sessionService;

        public ApiGameService(IHttpClientFactory httpClientFactory, SessionService sessionService)
        {
            _httpClient = httpClientFactory.CreateClient("GotchaApi");
            _sessionService = sessionService;
        }

        public async Task<ResultModel<List<GameItem>>> GetAllAsync()
        {
            ResultModel<List<GameItem>> result = new ResultModel<List<GameItem>>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/games");

                if (response.IsSuccessStatusCode)
                {
                    List<GameSummaryDto>? data = await response.Content.ReadFromJsonAsync<List<GameSummaryDto>>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    Guid currentUserId = _sessionService.CurrentUserId ?? Guid.Empty;

                    List<GameItem> items = new List<GameItem>();

                    foreach (GameSummaryDto dto in data)
                    {
                        items.Add(ToGameItem(dto, currentUserId));
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
                // but a siple Debug.WriteLine is sufficient for this example and easier to explain
                System.Diagnostics.Debug.WriteLine($"ApiGameService.GetAllAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<GameItem>> CreateAsync(GameRequestDto request)
        {
            ResultModel<GameItem> result = new ResultModel<GameItem>();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/games", request);

                if (response.IsSuccessStatusCode)
                {
                    GameResponseDto? data = await response.Content.ReadFromJsonAsync<GameResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    Guid currentUserId = _sessionService.CurrentUserId ?? Guid.Empty;

                    result.Data = ToGameItem(data, currentUserId);
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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.CreateAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<BaseResultModel> JoinAsync(Guid gameId)
        {
            BaseResultModel result = new BaseResultModel();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync($"api/games/{gameId}/join", content: null);

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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.JoinAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        private static GameItem ToGameItem(GameSummaryDto dto, Guid currentUserId)
        {
            return new GameItem
            {
                GameId = dto.Id,
                Name = dto.Name,
                CreationDateText = dto.CreationDate.ToString("dd MMM yyyy"),
                StartDateText = dto.StartDate?.ToString("dd MMM yyyy") ?? string.Empty,
                EndDateText = dto.EndDate?.ToString("dd MMM yyyy") ?? string.Empty,
                HasStarted = dto.HasStarted,
                IsFinished = dto.IsFinished,
                PlayerCount = dto.PlayerCount,
                KillCount = dto.KillCount,
                IsCreator = dto.CreatorId == currentUserId,
                CurrentUserPlayerId = dto.CurrentUserPlayerId,
                IsWinner = dto.WinnerId == currentUserId
            };
        }

        private static GameItem ToGameItem(GameResponseDto dto, Guid currentUserId)
        {
            return new GameItem
            {
                GameId = dto.Id,
                Name = dto.Name,
                CreationDateText = dto.CreationDate.ToString("dd MMM yyyy"),
                StartDateText = dto.StartDate?.ToString("dd MMM yyyy") ?? string.Empty,
                EndDateText = dto.EndDate?.ToString("dd MMM yyyy") ?? string.Empty,
                HasStarted = dto.HasStarted,
                IsFinished = dto.IsFinished,
                PlayerCount = dto.Players.Count,
                KillCount = dto.KillCount,
                IsCreator = dto.CreatorId == currentUserId,
                CurrentUserPlayerId = dto.Players.FirstOrDefault(p => p.UserId == currentUserId)?.Id,
                IsWinner = dto.WinnerId == currentUserId
            };
        }
    }
}
