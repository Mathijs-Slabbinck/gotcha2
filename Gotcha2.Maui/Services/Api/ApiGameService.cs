using System.Net;
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

        public async Task<ResultModel<GameDetailItem>> GetByIdAsync(Guid gameId)
        {
            ResultModel<GameDetailItem> result = new ResultModel<GameDetailItem>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/games/{gameId}");

                if (response.IsSuccessStatusCode)
                {
                    GameResponseDto? data = await response.Content.ReadFromJsonAsync<GameResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    Guid currentUserId = _sessionService.CurrentUserId ?? Guid.Empty;

                    result.Data = ToGameDetailItem(data, currentUserId);
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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.GetByIdAsync failed: {ex.Message}");

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

        public async Task<BaseResultModel> StartAsync(Guid gameId)
        {
            BaseResultModel result = new BaseResultModel();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync($"api/games/{gameId}/start", content: null);

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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.StartAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<BaseResultModel> LeaveAsync(Guid playerId)
        {
            BaseResultModel result = new BaseResultModel();

            try
            {
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/players/{playerId}");

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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.LeaveAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<List<KillItem>>> GetKillsAsync(Guid gameId)
        {
            ResultModel<List<KillItem>> result = new ResultModel<List<KillItem>>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/games/{gameId}/kills");

                if (response.IsSuccessStatusCode)
                {
                    List<KillSummaryDto>? data = await response.Content.ReadFromJsonAsync<List<KillSummaryDto>>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    List<KillItem> items = new List<KillItem>();

                    foreach (KillSummaryDto dto in data)
                    {
                        items.Add(ToKillItem(dto, gameId));
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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.GetKillsAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<KillItem>> RecordKillAsync(Guid gameId, KillRequestDto request)
        {
            ResultModel<KillItem> result = new ResultModel<KillItem>();

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"api/games/{gameId}/kills", request);

                if (response.IsSuccessStatusCode)
                {
                    KillResponseDto? data = await response.Content.ReadFromJsonAsync<KillResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    result.Data = ToKillItem(data);
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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.RecordKillAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        public async Task<ResultModel<TargetAssignmentItem?>> GetMyTargetAsync(Guid gameId)
        {
            ResultModel<TargetAssignmentItem?> result = new ResultModel<TargetAssignmentItem?>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/games/{gameId}/my-target");

                if (response.IsSuccessStatusCode)
                {
                    TargetAssignmentResponseDto? data = await response.Content.ReadFromJsonAsync<TargetAssignmentResponseDto>();

                    if (data is null)
                    {
                        result.Errors.Add("Empty response from server.");
                        return result;
                    }

                    result.Data = ToTargetAssignmentItem(data);
                    return result;
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // No open assignment (caller is dead, between assignments, or game not started yet).
                    // Surface as success + null data so the UI can hide the target card without showing an error.
                    result.Data = null;
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
                System.Diagnostics.Debug.WriteLine($"ApiGameService.GetMyTargetAsync failed: {ex.Message}");

                result.Errors.Add("Could not reach the server. Please check your connection.");
                return result;
            }
        }

        private static KillItem ToKillItem(KillSummaryDto dto, Guid gameId)
        {
            return new KillItem
            {
                KillId = dto.Id,
                GameId = gameId,
                MomentText = dto.Moment.ToString("dd MMM yyyy HH:mm"),
                KillerUserId = dto.Killer.UserId,
                KillerDisplayName = $"{dto.Killer.FirstName} {dto.Killer.LastName}",
                VictimUserId = dto.Victim.UserId,
                VictimDisplayName = $"{dto.Victim.FirstName} {dto.Victim.LastName}"
            };
        }

        private static KillItem ToKillItem(KillResponseDto dto)
        {
            return new KillItem
            {
                KillId = dto.Id,
                GameId = dto.GameId,
                MomentText = dto.Moment.ToString("dd MMM yyyy HH:mm"),
                KillerUserId = dto.Killer.UserId,
                KillerDisplayName = $"{dto.Killer.FirstName} {dto.Killer.LastName}",
                VictimUserId = dto.Victim.UserId,
                VictimDisplayName = $"{dto.Victim.FirstName} {dto.Victim.LastName}"
            };
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

        private static GameDetailItem ToGameDetailItem(GameResponseDto dto, Guid currentUserId)
        {
            List<PlayerItem> players = new List<PlayerItem>();

            foreach (PlayerSummaryDto player in dto.Players)
            {
                players.Add(ToPlayerItem(player));
            }

            return new GameDetailItem
            {
                GameId = dto.Id,
                Name = dto.Name,
                CreationDateText = dto.CreationDate.ToString("dd MMM yyyy"),
                StartDateText = dto.StartDate?.ToString("dd MMM yyyy") ?? string.Empty,
                EndDateText = dto.EndDate?.ToString("dd MMM yyyy") ?? string.Empty,
                HasStarted = dto.HasStarted,
                IsFinished = dto.IsFinished,
                KillCount = dto.KillCount,
                IsCreator = dto.CreatorId == currentUserId,
                CreatorId = dto.CreatorId,
                WinnerId = dto.WinnerId,
                CurrentUserPlayerId = dto.Players.FirstOrDefault(p => p.UserId == currentUserId)?.Id,
                IsWinner = dto.WinnerId == currentUserId,
                Players = players
            };
        }

        // Duplicated from ApiPlayerService.ToPlayerItem on purpose — per-service encapsulation
        // beats spinning up a shared helper for two callers. Extract if a third consumer ever lands.
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

        private static TargetAssignmentItem ToTargetAssignmentItem(TargetAssignmentResponseDto dto)
        {
            return new TargetAssignmentItem
            {
                AssignmentId = dto.Id,
                GameId = dto.GameId,
                AssignedDateText = dto.TargetAssigned.ToString("dd MMM yyyy HH:mm"),
                TargetPlayerId = dto.Target.Id,
                TargetUserId = dto.Target.UserId,
                TargetDisplayName = $"{dto.Target.FirstName} {dto.Target.LastName}",
                TargetHasProfileImage = dto.Target.HasProfileImage
            };
        }
    }
}
