using Gotcha2.API.Dtos.Players.Response;
using Gotcha2.API.Dtos.Players.Response.Summary;
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
                FirstName = player.User.FirstName,
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
                FirstName = player.User.FirstName,
                LastName = player.User.LastName,
                HasProfileImage = player.User.HasProfileImage,
                GameId = player.GameId,
                GameName = player.Game.Name,
                IsAlive = player.IsAlive,
                Notes = player.Notes
            };
        }
    }
}
