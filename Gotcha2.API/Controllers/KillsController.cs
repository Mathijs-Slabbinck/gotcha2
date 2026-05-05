using Gotcha2.API.Dtos.Kills.Request;
using Gotcha2.API.Dtos.Kills.Response;
using Gotcha2.API.Dtos.Kills.Response.Summary;
using Gotcha2.API.Services.Helpers.Extensions;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId:guid}/kills")]
    [Authorize]
    public class KillsController : ControllerBase
    {
        private readonly IKillRepoService _killRepo;
        private readonly IGameRepoService _gameRepo;

        public KillsController(IKillRepoService killRepo, IGameRepoService gameRepo)
        {
            _killRepo = killRepo;
            _gameRepo = gameRepo;
        }

        #region === GET ALL BY GAME ===

        [HttpGet]
        public async Task<IActionResult> GetByGame(Guid gameId)
        {
            // Caller must be a Player in the game.
            ResultModel<Game> gameResult = await _gameRepo.GetByIdAsync(gameId);

            if (!gameResult.IsSuccess)
            {
                return NotFound(gameResult.Errors);
            }

            Guid currentUserId = User.GetUserId();
            bool isInGame = gameResult.Data!.Players.Any(p => p.UserId == currentUserId);

            if (!isInGame)
            {
                return Forbid();
            }

            ResultModel<List<Kill>> killsResult = await _killRepo.GetByGameAsync(gameId);

            if (!killsResult.IsSuccess)
            {
                return BadRequest(killsResult.Errors);
            }

            List<KillSummaryDto> response = killsResult.Data!
                .Select(k => k.MapToKillSummaryDto())
                .ToList();

            return Ok(response);
        }

        #endregion

        #region === POST (record a kill) ===

        [HttpPost]
        public async Task<IActionResult> Create(Guid gameId, [FromBody] KillRequestDto dto)
        {
            Guid currentUserId = User.GetUserId();
            ResultModel<Kill> result = await _killRepo.RecordKillAsync(gameId, dto.VictimPlayerId!.Value, currentUserId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            KillResponseDto response = result.Data!.MapToResponseDto();
            return StatusCode(StatusCodes.Status201Created, response);
        }

        #endregion
    }
}
