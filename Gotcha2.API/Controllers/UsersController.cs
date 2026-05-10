using Gotcha2.API.Constants;
using Gotcha2.API.Dtos.Users.Request;
using Gotcha2.API.Dtos.Users.Response;
using Gotcha2.API.Services.Helpers.Extensions;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Gotcha2.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly UserManager<GotchaUser> _userManager;
        private readonly IPlayerRepoService _playerRepo;
        private readonly IGameRepoService _gameRepo;
        private readonly IKillRepoService _killRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(
            UserManager<GotchaUser> userManager,
            IPlayerRepoService playerRepo,
            IGameRepoService gameRepo,
            IKillRepoService killRepo,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _playerRepo = playerRepo;
            _gameRepo = gameRepo;
            _killRepo = killRepo;
            _webHostEnvironment = webHostEnvironment;
        }

        #region === GET /api/users/me ===

        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            Guid currentUserId = User.GetUserId();
            GotchaUser? user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user == null)
                return NotFound(new[] { "User not found." });

            ResultModel<int> gamesPlayedResult = await _playerRepo.CountByUserAsync(currentUserId);
            if (!gamesPlayedResult.IsSuccess)
                return BadRequest(gamesPlayedResult.Errors);

            ResultModel<int> gamesWonResult = await _gameRepo.CountWinsByUserAsync(currentUserId);
            if (!gamesWonResult.IsSuccess)
                return BadRequest(gamesWonResult.Errors);

            ResultModel<int> totalKillsResult = await _killRepo.CountByKillerUserAsync(currentUserId);
            if (!totalKillsResult.IsSuccess)
                return BadRequest(totalKillsResult.Errors);

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
                GamesPlayed = gamesPlayedResult.Data,
                GamesWon = gamesWonResult.Data,
                TotalKills = totalKillsResult.Data
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
                return NotFound(new[] { "User not found." });

            // Email-change path: also rotate UserName so login (which keys off Email) keeps working.
            bool emailChanged = !string.Equals(user.Email, dto.Email, StringComparison.OrdinalIgnoreCase);

            if (emailChanged)
            {
                GotchaUser? clash = await _userManager.FindByEmailAsync(dto.Email);

                if (clash != null && clash.Id != currentUserId)
                    return BadRequest(new[] { "A user with that email already exists." });

                user.Email = dto.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(dto.Email);
                user.UserName = dto.Email;
                user.NormalizedUserName = _userManager.NormalizeName(dto.Email);
            }

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            IdentityResult updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description).ToArray());

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
                return NotFound(new[] { "User not found." });

            // No current-password check — this endpoint sets a new password directly.
            // RemovePassword + AddPassword is the documented way to bypass the old-password requirement.
            IdentityResult removeResult = await _userManager.RemovePasswordAsync(user);

            if (!removeResult.Succeeded)
                return BadRequest(removeResult.Errors.Select(e => e.Description).ToArray());

            IdentityResult addResult = await _userManager.AddPasswordAsync(user, dto.NewPassword);

            if (!addResult.Succeeded)
                return BadRequest(addResult.Errors.Select(e => e.Description).ToArray());

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
                return NotFound(new[] { "User not found." });

            if (!user.HasProfileImage)
                return NotFound(new[] { "User has no profile image." });

            string folder = Path.Combine(_webHostEnvironment.WebRootPath, ProfileImageStorage.FolderName);
            string filePath = Path.Combine(folder, id.ToString() + ProfileImageStorage.FileExtension);

            try
            {
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(bytes, ProfileImageStorage.ContentType);
            }
            catch (FileNotFoundException)
            {
                // Self-heal: flag and disk are out of sync (file deleted out-of-band). Flip the flag back so the
                // client doesn't keep retrying. This is an intentional write on a GET — only fires on the broken
                // path, not on every read, and once flipped the early-return at the HasProfileImage check above
                // short-circuits future requests.
                user.HasProfileImage = false;
                await _userManager.UpdateAsync(user);
                return NotFound(new[] { "User has no profile image." });
            }
        }

        #endregion

        #region === PUT /api/users/me/profile-image ===

        [HttpPut("me/profile-image")]
        [RequestSizeLimit(MaxImageBytes)]
        // Under [ApiController], parameters get an inferred binding source:
        // - Complex types → [FromBody] (JSON)
        // - Route-template matches → [FromRoute]
        // - Everything else → [FromQuery]
        // But this is a convention, not a guarantee so we add [FromForm] to be explicit and avoid any issues with multipart/form-data.
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new[] { "No file uploaded." });

            if (file.Length > MaxImageBytes)
                return BadRequest(new[] { "File exceeds the 5 MB limit." });

            Guid currentUserId = User.GetUserId();
            GotchaUser? user = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (user == null)
                return NotFound(new[] { "User not found." });

            string folder = Path.Combine(_webHostEnvironment.WebRootPath, ProfileImageStorage.FolderName);
            Directory.CreateDirectory(folder);

            string filePath = Path.Combine(folder, currentUserId.ToString() + ProfileImageStorage.FileExtension);

            try
            {
                // Reads the uploaded bytes from the HTTP request (in memory / temp file).
                await using Stream input = file.OpenReadStream();
                // Decodes those bytes into an in-memory bitmap (an ImageSharp Image object).
                // "Image" is just a C# class living in RAM — nothing on disk yet, nothing in the DB.
                using Image image = await Image.LoadAsync(input);
                // Opens a file on disk at wwwroot/profile-images/{userId}.jpg for writing.
                await using FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                // Encodes the bitmap as JPEG bytes and writes them into that FileStream → the .jpg file on disk.
                await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = 90 });
            }
            catch (UnknownImageFormatException)
            {
                return BadRequest(new[] { "File is not a valid image." });
            }
            catch (InvalidImageContentException)
            {
                return BadRequest(new[] { "File is not a valid image." });
            }

            if (!user.HasProfileImage)
            {
                user.HasProfileImage = true;
                IdentityResult flagResult = await _userManager.UpdateAsync(user);

                if (!flagResult.Succeeded)
                    return BadRequest(flagResult.Errors.Select(e => e.Description).ToArray());
            }

            return NoContent();
        }

        #endregion
    }
}