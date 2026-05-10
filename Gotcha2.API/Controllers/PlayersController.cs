using Gotcha2.API.Dtos.Players.Response.Summary;
using Gotcha2.API.Services.Helpers.Extensions;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    [Authorize]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerRepoService _playerRepo;

        public PlayersController(IPlayerRepoService playerRepo)
        {
            _playerRepo = playerRepo;
        }

        #region === GET ALL BY GAME ===

        [HttpGet("/api/games/{gameId:guid}/players")]
        public async Task<IActionResult> GetByGame(Guid gameId)
        {
            // Caller must be a Player in the game.
            ResultModel<List<Player>> playersResult = await _playerRepo.GetByGameAsync(gameId);

            if (!playersResult.IsSuccess)
                return NotFound(playersResult.Errors);

            Guid currentUserId = User.GetUserId();

            if (!playersResult.Data!.IsMember(currentUserId))
                return Forbid();

            List<PlayerSummaryDto> response = playersResult.Data!
                                                            .Select(p => p.MapToPlayerSummaryDto())
                                                            .ToList();

            return Ok(response);
        }

        #endregion

        #region === GET BY ID ===

        [HttpGet("/api/players/{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            ResultModel<Player> result = await _playerRepo.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            // Caller must share a game with the requested player.
            Guid currentUserId = User.GetUserId();
            ResultModel<bool> membershipResult = await _playerRepo.IsUserInGameAsync(result.Data!.GameId, currentUserId);

            if (!membershipResult.IsSuccess)
                return BadRequest(membershipResult.Errors);

            if (!membershipResult.Data)
                return Forbid();

            return Ok(result.Data!.MapToResponseDto());
        }

        #endregion

        #region === DELETE (leave game) ===

        [HttpDelete("/api/players/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            ResultModel<Player> existing = await _playerRepo.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(existing.Errors);

            Guid currentUserId = User.GetUserId();

            // Only the owner can leave (delete their own player row).
            if (existing.Data!.UserId != currentUserId)
                return Forbid();

            // Game must not have started.
            if (existing.Data!.Game!.HasStarted)
                return BadRequest(new[] { "Cannot leave a game that has already started." });

            ResultModel<Player> deleteResult = await _playerRepo.DeleteAsync(id);

            if (!deleteResult.IsSuccess)
                return BadRequest(deleteResult.Errors);

            return NoContent();
        }

        #endregion
    }
}
