using Gotcha2.API.Dtos.TargetAssignments.Response;
using Gotcha2.API.Services.Helpers.Extensions;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    [Route("api/games/{gameId:guid}/my-target")]
    [Authorize]
    public class TargetAssignmentsController : ControllerBase
    {
        private readonly ITargetAssignmentRepoService _targetRepo;

        public TargetAssignmentsController(ITargetAssignmentRepoService targetRepo)
        {
            _targetRepo = targetRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTarget(Guid gameId)
        {
            Guid currentUserId = User.GetUserId();
            ResultModel<TargetAssignment?> result = await _targetRepo.GetMyTargetAsync(gameId, currentUserId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Errors);
            }

            if (result.Data == null)
            {
                return NotFound(new[] { "You have no open target assignment in this game." });
            }

            return Ok(result.Data.MapToResponseDto());
        }
    }
}
