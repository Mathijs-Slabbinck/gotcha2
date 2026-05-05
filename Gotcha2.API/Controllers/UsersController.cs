using Gotcha2.API.Constants;
using Gotcha2.API.Dtos.Users.Request;
using Gotcha2.API.Dtos.Users.Response;
using Gotcha2.API.Services.Helpers.Extensions;
using Gotcha2.Core.Data;
using Gotcha2.Core.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly UserManager<GotchaUser> _userManager;
        private readonly Gotcha2DBContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(
            UserManager<GotchaUser> userManager,
            Gotcha2DBContext dbContext,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        #region === GET /api/users/me ===

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            Guid currentUserId = User.GetUserId();
            GotchaUser? user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user == null)
            {
                return NotFound(new[] { "User not found." });
            }

            // Computed stats — gamesPlayed = #player rows; gamesWon = #games where this user is winner; totalKills = #kills as killer.
            int gamesPlayed = await _dbContext.Players.CountAsync(p => p.UserId == currentUserId);
            int gamesWon = await _dbContext.Games.CountAsync(g => g.IsFinished && g.WinnerId == currentUserId);
            int totalKills = await _dbContext.Kills.CountAsync(k => k.Killer.UserId == currentUserId);

            UserResponseDto response = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                AccountCreationDate = user.AccountCreationDate,
                HasProfileImage = user.HasProfileImage,
                GamesPlayed = gamesPlayed,
                GamesWon = gamesWon,
                TotalKills = totalKills
            };

            return Ok(response);
        }

        #endregion

        #region === PUT /api/users/me ===

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateRequestDto dto)
        {
            Guid currentUserId = User.GetUserId();
            GotchaUser? user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user == null)
            {
                return NotFound(new[] { "User not found." });
            }

            // Email-change path: also rotate UserName so login (which keys off Email) keeps working.
            bool emailChanged = !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase);

            if (emailChanged)
            {
                GotchaUser? clash = await _userManager.FindByEmailAsync(dto.Email);

                if (clash != null && clash.Id != currentUserId)
                {
                    return BadRequest(new[] { "A user with that email already exists." });
                }

                IdentityResult emailResult = await _userManager.SetEmailAsync(user, dto.Email);

                if (!emailResult.Succeeded)
                {
                    return BadRequest(emailResult.Errors.Select(e => e.Description).ToArray());
                }

                IdentityResult userNameResult = await _userManager.SetUserNameAsync(user, dto.Email);

                if (!userNameResult.Succeeded)
                {
                    return BadRequest(userNameResult.Errors.Select(e => e.Description).ToArray());
                }
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            IdentityResult updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors.Select(e => e.Description).ToArray());
            }

            return await GetMe();
        }

        #endregion

        #region === PUT /api/users/me/password ===

        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto dto)
        {
            Guid currentUserId = User.GetUserId();
            GotchaUser? user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user == null)
            {
                return NotFound(new[] { "User not found." });
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description).ToArray());
            }

            return NoContent();
        }

        #endregion

        #region === GET /api/users/{id}/profile-image ===

        [HttpGet("{id:guid}/profile-image")]
        public async Task<IActionResult> GetProfileImage(Guid id)
        {
            // Any logged-in user can read any user's profile image — needed to render target / killer / player rows.
            GotchaUser? user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return NotFound(new[] { "User not found." });
            }

            if (!user.HasProfileImage)
            {
                return NotFound(new[] { "User has no profile image." });
            }

            string folder = Path.Combine(_webHostEnvironment.WebRootPath, ProfileImageStorage.FolderName);
            string filePath = Path.Combine(folder, id.ToString() + ProfileImageStorage.FileExtension);

            if (!System.IO.File.Exists(filePath))
            {
                // Flag and disk are out of sync — flip the flag back so the client doesn't keep retrying.
                user.HasProfileImage = false;
                await _userManager.UpdateAsync(user);
                return NotFound(new[] { "User has no profile image." });
            }

            byte[] bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, ProfileImageStorage.ContentType);
        }

        #endregion

        #region === PUT /api/users/me/profile-image ===

        [HttpPut("me/profile-image")]
        [RequestSizeLimit(MaxImageBytes)]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new[] { "No file uploaded." });
            }

            if (file.Length > MaxImageBytes)
            {
                return BadRequest(new[] { "File exceeds the 5 MB limit." });
            }

            Guid currentUserId = User.GetUserId();
            GotchaUser? user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user == null)
            {
                return NotFound(new[] { "User not found." });
            }

            string folder = Path.Combine(_webHostEnvironment.WebRootPath, ProfileImageStorage.FolderName);
            Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, currentUserId.ToString() + ProfileImageStorage.FileExtension);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream);
            }

            if (!user.HasProfileImage)
            {
                user.HasProfileImage = true;
                IdentityResult flagResult = await _userManager.UpdateAsync(user);

                if (!flagResult.Succeeded)
                {
                    return BadRequest(flagResult.Errors.Select(e => e.Description).ToArray());
                }
            }

            return NoContent();
        }

        #endregion
    }
}
