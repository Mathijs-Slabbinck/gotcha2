using Gotcha2.API.Dtos.Games.Request;
using Gotcha2.API.Dtos.Games.Response;
using Gotcha2.API.Dtos.Games.Response.Summary;
using Gotcha2.API.Services.Helpers.Extensions;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IGameRepoService _gameRepo;

        public GamesController(IGameRepoService gameRepo)
        {
            _gameRepo = gameRepo;
        }

        #region === GET ALL (only games the caller is in) ===

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            Guid currentUserId = User.GetUserId();
            ResultModel<List<Game>> result = await _gameRepo.GetByUserAsync(currentUserId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            List<GameSummaryDto> response = result.Data!
                                                    .Select(g => g.MapToGameSummaryDto())
                                                    .ToList();

            return Ok(response);
        }

        #endregion

        #region === GET BY ID ===

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            ResultModel<Game> result = await _gameRepo.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            Guid currentUserId = User.GetUserId();

            if (!result.Data!.IsMember(currentUserId))
                return Forbid();

            return Ok(result.Data!.MapToResponseDto());
        }

        #endregion

        #region === POST (create) ===

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] GameRequestDto dto)
        {
            Guid currentUserId = User.GetUserId();

            // Build the Game with the creator's first Player attached so EF inserts both in one SaveChanges.
            Game game = new Game
            {
                Name = dto.Name,
                CreatorId = currentUserId
            };

            Player creatorPlayer = new Player
            {
                UserId = currentUserId,
                GameId = game.Id,
                IsAlive = true
            };

            game.Players.Add(creatorPlayer);

            ResultModel<Game> addResult = await _gameRepo.AddAsync(game);

            if (!addResult.IsSuccess)
                return BadRequest(addResult.Errors);

            // GameRepoService.AddAsync reloads the full graph internally, so addResult.Data is mappable.
            GameResponseDto response = addResult.Data!.MapToResponseDto();

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        #endregion

        #region === PUT (rename — creator only, before start) ===

        // Reference convention: PUT goes to /api/{resource} with id in body, not in URL. Matches reference's ConcertsController / TicketsController.
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] GameUpdateRequestDto dto)
        {
            Guid id = dto.Id!.Value;
            ResultModel<Game> existing = await _gameRepo.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(existing.Errors);

            Guid currentUserId = User.GetUserId();

            if (existing.Data!.CreatorId != currentUserId)
                return Forbid();

            if (existing.Data!.HasStarted)
                return BadRequest(new[] { "Cannot rename a game that has already started." });

            Game updated = new Game
            {
                Id = id,
                Name = dto.Name
            };

            ResultModel<Game> result = await _gameRepo.UpdateAsync(updated);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            // GameRepoService.UpdateAsync loads the full graph internally, so result.Data is mappable.
            return Ok(result.Data!.MapToResponseDto());
        }

        #endregion

        #region === DELETE (creator only, only the creator's player row remaining) ===

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            ResultModel<Game> existing = await _gameRepo.GetByIdAsync(id);

            if (!existing.IsSuccess)
                return NotFound(existing.Errors);

            Guid currentUserId = User.GetUserId();

            if (existing.Data!.CreatorId != currentUserId)
                return Forbid();

            // Allow only when nobody but the creator has joined (otherwise risks deleting other people's data).
            bool hasOtherPlayers = existing.Data!.Players.Any(p => p.UserId != currentUserId);

            if (hasOtherPlayers)
                return BadRequest(new[] { "Cannot delete a game that other players have joined." });

            ResultModel<Game> result = await _gameRepo.DeleteAsync(id);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return NoContent();
        }

        #endregion

        #region === JOIN ===

        [HttpPost("{id:guid}/join")]
        public async Task<IActionResult> Join(Guid id)
        {
            Guid currentUserId = User.GetUserId();
            ResultModel<Game> result = await _gameRepo.JoinAsync(id, currentUserId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Data!.MapToResponseDto());
        }

        #endregion

        #region === START ===

        [HttpPost("{id:guid}/start")]
        public async Task<IActionResult> Start(Guid id)
        {
            Guid currentUserId = User.GetUserId();
            ResultModel<Game> result = await _gameRepo.StartAsync(id, currentUserId);

            if (!result.IsSuccess)
                return BadRequest(result.Errors);

            return Ok(result.Data!.MapToResponseDto());
        }

        #endregion
    }
}
