using Gotcha2.Core.Data;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.Core.Services.Repository
{
    public class PlayerRepoService : IPlayerRepoService
    {
        private readonly Gotcha2DBContext _dbContext;

        public PlayerRepoService(Gotcha2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<Player>> GetByIdAsync(Guid id)
        {
            ResultModel<Player> resultModel = new ResultModel<Player>();

            try
            {
                Player? player = await _dbContext.Players
                                                    .AsNoTracking()
                                                    .Include(p => p.User)
                                                    .Include(p => p.Game)
                                                    .FirstOrDefaultAsync(p => p.Id == id);

                if (player == null)
                {
                    resultModel.Errors.Add($"No player found with id {id}!");
                    return resultModel;
                }

                resultModel.Data = player;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch the player!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to fetch the player!");
                return resultModel;
            }
        }

        public async Task<ResultModel<List<Player>>> GetByGameAsync(Guid gameId)
        {
            ResultModel<List<Player>> resultModel = new ResultModel<List<Player>>();

            try
            {
                List<Player> players = await _dbContext.Players
                                                            .AsNoTracking()
                                                            .Include(p => p.User)
                                                            .Where(p => p.GameId == gameId)
                                                            .ToListAsync();
                resultModel.Data = players;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch the players!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to fetch the players!");
                return resultModel;
            }
        }

        public async Task<ResultModel<bool>> IsUserInGameAsync(Guid gameId, Guid userId)
        {
            ResultModel<bool> resultModel = new ResultModel<bool>();

            try
            {
                resultModel.Data = await _dbContext.Players
                                                        .AnyAsync(p => p.GameId == gameId && p.UserId == userId);
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while checking game membership!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while checking game membership!");
                return resultModel;
            }
        }

        public async Task<ResultModel<Player>> DeleteAsync(Guid id)
        {
            ResultModel<Player> resultModel = new ResultModel<Player>();

            try
            {
                Player? player = await _dbContext.Players.FirstOrDefaultAsync(p => p.Id == id);

                if (player == null)
                {
                    resultModel.Errors.Add("No player with this id was found!");
                    return resultModel;
                }

                _dbContext.Players.Remove(player);
                await _dbContext.SaveChangesAsync();
                resultModel.Data = player;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to delete the player!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to delete the player!");
                return resultModel;
            }
        }

        public async Task<ResultModel<int>> CountByUserAsync(Guid userId)
        {
            ResultModel<int> resultModel = new ResultModel<int>();

            try
            {
                resultModel.Data = await _dbContext.Players.CountAsync(p => p.UserId == userId);
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to count the players!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to count the players!");
                return resultModel;
            }
        }
    }
}
