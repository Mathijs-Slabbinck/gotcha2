using Gotcha2.API.Dtos.Games.Response;
using Gotcha2.API.Dtos.Games.Response.Summary;
using Gotcha2.API.Dtos.Kills.Response;
using Gotcha2.API.Dtos.Kills.Response.Summary;
using Gotcha2.API.Dtos.Players.Response;
using Gotcha2.API.Dtos.Players.Response.Summary;
using Gotcha2.API.Dtos.TargetAssignments.Response;
using Gotcha2.Core.Entities.Models;

namespace Gotcha2.API.Services.Helpers.Extensions
{
    // Central entity → DTO mapping. Methods are added per-resource as Phase 2 progresses.
    // Naming convention (matches reference): `MapToResponseDto` is overloaded across entities (compiler
    // disambiguates by the `this` parameter type), but Summary methods get unique names
    // (`MapToPlayerSummaryDto`, `MapToGameSummaryDto`, `MapToKillSummaryDto`) so callers don't have to
    // wrap them in lambdas to disambiguate.
    public static class MappingExtensions
    {
        public static PlayerSummaryDto MapToPlayerSummaryDto(this Player player)
        {
            return new PlayerSummaryDto
            {
                Id = player.Id,
                UserId = player.UserId,
                FirstName = player.User!.FirstName,
                LastName = player.User.LastName,
                IsAlive = player.IsAlive,
                HasProfileImage = player.User.HasProfileImage
            };
        }

        public static PlayerResponseDto MapToResponseDto(this Player player)
        {
            return new PlayerResponseDto
            {
                Id = player.Id,
                UserId = player.UserId,
                FirstName = player.User!.FirstName,
                LastName = player.User.LastName,
                HasProfileImage = player.User.HasProfileImage,
                GameId = player.GameId,
                GameName = player.Game!.Name,
                IsAlive = player.IsAlive,
                Notes = player.Notes
            };
        }

        public static GameResponseDto MapToResponseDto(this Game game)
        {
            return new GameResponseDto
            {
                Id = game.Id,
                Name = game.Name,
                CreationDate = game.CreationDate,
                StartDate = game.StartDate,
                EndDate = game.EndDate,
                HasStarted = game.HasStarted,
                IsFinished = game.IsFinished,
                WinnerId = game.WinnerId,
                CreatorId = game.CreatorId,
                Players = game.Players.Select(MapToPlayerSummaryDto).ToList(),
                KillCount = game.Kills.Count
            };
        }

        public static GameSummaryDto MapToGameSummaryDto(this Game game)
        {
            return new GameSummaryDto
            {
                Id = game.Id,
                Name = game.Name,
                CreationDate = game.CreationDate,
                StartDate = game.StartDate,
                EndDate = game.EndDate,
                HasStarted = game.HasStarted,
                IsFinished = game.IsFinished,
                WinnerId = game.WinnerId,
                CreatorId = game.CreatorId,
                PlayerCount = game.Players.Count
            };
        }

        public static KillResponseDto MapToResponseDto(this Kill kill)
        {
            return new KillResponseDto
            {
                Id = kill.Id,
                GameId = kill.GameId,
                Moment = kill.Moment,
                Killer = kill.Killer!.MapToPlayerSummaryDto(),
                Victim = kill.Victim!.MapToPlayerSummaryDto()
            };
        }

        public static KillSummaryDto MapToKillSummaryDto(this Kill kill)
        {
            return new KillSummaryDto
            {
                Id = kill.Id,
                Moment = kill.Moment,
                Killer = kill.Killer!.MapToPlayerSummaryDto(),
                Victim = kill.Victim!.MapToPlayerSummaryDto()
            };
        }

        public static TargetAssignmentResponseDto MapToResponseDto(this TargetAssignment assignment)
        {
            return new TargetAssignmentResponseDto
            {
                Id = assignment.Id,
                GameId = assignment.GameId,
                TargetAssigned = assignment.TargetAssigned,
                Hunter = assignment.Hunter!.MapToPlayerSummaryDto(),
                Target = assignment.Target!.MapToPlayerSummaryDto()
            };
        }
    }
}
