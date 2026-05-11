using Gotcha2.Core.Data;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.Core.Services.Repository
{
    public class GameRepoService : IGameRepoService
    {
        private readonly Gotcha2DBContext _dbContext;

        public GameRepoService(Gotcha2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region === CRUD ===

        public async Task<ResultModel<Game>> GetByIdAsync(Guid id)
        {
            ResultModel<Game> resultModel = new ResultModel<Game>();

            try
            {
                Game? game = await GamesWithDetailsReadOnly().FirstOrDefaultAsync(g => g.Id == id);

                if (game == null)
                {
                    resultModel.Errors.Add($"No game found with id {id}!");
                    return resultModel;
                }

                resultModel.Data = game;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch the game!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to fetch the game!");
                return resultModel;
            }
        }

        public async Task<ResultModel<Game>> AddAsync(Game game)
        {
            ResultModel<Game> resultModel = new ResultModel<Game>();

            try
            {
                await _dbContext.Games.AddAsync(game);
                await _dbContext.SaveChangesAsync();

                // Reload with the full graph so callers can map without a second round-trip.
                // The User nav on the freshly-inserted creator Player is null otherwise, and mapping NPEs.
                Game? reloaded = await GamesWithDetailsTracked().FirstOrDefaultAsync(g => g.Id == game.Id);

                resultModel.Data = reloaded!;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to add the game!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to add the game!");
                return resultModel;
            }
        }

        public async Task<ResultModel<Game>> UpdateAsync(Game game)
        {
            ResultModel<Game> resultModel = new ResultModel<Game>();

            try
            {
                // Load with the full graph upfront so the returned entity is mappable without a reload
                // (matches reference ConcertRepoService.UpdateAsync — loads Artists + Tickets up front).
                Game? existing = await GamesWithDetailsTracked().FirstOrDefaultAsync(g => g.Id == game.Id);

                if (existing == null)
                {
                    resultModel.Errors.Add("No game with this id was found!");
                    return resultModel;
                }

                existing.Name = game.Name;

                await _dbContext.SaveChangesAsync();
                resultModel.Data = existing;
                return resultModel;
            }
            catch (DbUpdateConcurrencyException)
            {
                resultModel.Errors.Add("The game you are trying to update has already been updated!");
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to update the game!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to update the game!");
                return resultModel;
            }
        }

        public async Task<ResultModel<Game>> DeleteAsync(Guid id)
        {
            ResultModel<Game> resultModel = new ResultModel<Game>();

            try
            {
                Game? game = await _dbContext.Games.FirstOrDefaultAsync(g => g.Id == id);

                if (game == null)
                {
                    resultModel.Errors.Add("No game with this id was found!");
                    return resultModel;
                }

                _dbContext.Games.Remove(game);
                await _dbContext.SaveChangesAsync();
                resultModel.Data = game;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to delete the game!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to delete the game!");
                return resultModel;
            }
        }

        #endregion

        #region === HELPERS ===

        private IQueryable<Game> GamesWithDetailsTracked() => _dbContext.Games
                                                                    .Include(g => g.Players)
                                                                        .ThenInclude(p => p.User)
                                                                    .Include(g => g.Kills)
                                                                    .AsSplitQuery();

        private IQueryable<Game> GamesWithDetailsReadOnly() => GamesWithDetailsTracked().AsNoTracking();
        #endregion

        #region === DOMAIN OPS ===

        public async Task<ResultModel<List<Game>>> GetByUserAsync(Guid userId)
        {
            ResultModel<List<Game>> resultModel = new ResultModel<List<Game>>();

            try
            {
                List<Game> games = await GamesWithDetailsReadOnly()
                                                .Where(g => g.Players
                                                                .Any(p => p.UserId == userId))
                                                .OrderByDescending(g => g.CreationDate)
                                                .ToListAsync();

                resultModel.Data = games;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch your games!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to fetch your games!");
                return resultModel;
            }
        }

        public async Task<ResultModel<Game>> JoinAsync(Guid gameId, Guid userId)
        {
            ResultModel<Game> resultModel = new ResultModel<Game>();

            try
            {
                Game? game = await _dbContext.Games
                                                .Include(g => g.Players)
                                                .FirstOrDefaultAsync(g => g.Id == gameId);

                if (game == null)
                {
                    resultModel.Errors.Add("No game with this id was found!");
                    return resultModel;
                }

                if (game.HasStarted)
                {
                    resultModel.Errors.Add("Cannot join a game that has already started.");
                    return resultModel;
                }

                if (game.IsFinished)
                {
                    resultModel.Errors.Add("Cannot join a game that has already finished.");
                    return resultModel;
                }

                bool alreadyJoined = game.Players.Any(p => p.UserId == userId);

                if (alreadyJoined)
                {
                    resultModel.Errors.Add("You have already joined this game.");
                    return resultModel;
                }

                Player newPlayer = new Player
                {
                    UserId = userId,
                    GameId = gameId,
                    IsAlive = true
                };

                await _dbContext.Players.AddAsync(newPlayer);
                await _dbContext.SaveChangesAsync();

                // Reload to get the user and updated player list materialised for mapping.
                Game? reloaded = await GamesWithDetailsTracked().FirstOrDefaultAsync(g => g.Id == gameId);

                resultModel.Data = reloaded!;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to join the game!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to join the game!");
                return resultModel;
            }
        }

        public async Task<ResultModel<Game>> StartAsync(Guid gameId, Guid creatorUserId)
        {
            ResultModel<Game> resultModel = new ResultModel<Game>();

            try
            {
                Game? game = await _dbContext.Games
                                                .Include(g => g.Players)
                                                .FirstOrDefaultAsync(g => g.Id == gameId);

                if (game == null)
                {
                    resultModel.Errors.Add("No game with this id was found!");
                    return resultModel;
                }

                if (game.CreatorId != creatorUserId)
                {
                    resultModel.Errors.Add("Only the creator can start the game.");
                    return resultModel;
                }

                if (game.HasStarted)
                {
                    resultModel.Errors.Add("Game has already started.");
                    return resultModel;
                }

                if (game.Players.Count < 2)
                {
                    resultModel.Errors.Add("A game needs at least 2 players to start.");
                    return resultModel;
                }

                // Shuffle the players into a ring: shuffled[i] hunts shuffled[(i + 1) % count].
                List<Player> shuffled = game.Players.ToList();
                int playerCount = shuffled.Count;
                for (int i = playerCount - 1; i > 0; i--)
                {
                    int j = Random.Shared.Next(i + 1);
                    (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
                }

                for (int i = 0; i < playerCount; i++)
                {
                    Player hunter = shuffled[i];
                    Player target = shuffled[(i + 1) % playerCount];

                    TargetAssignment assignment = new TargetAssignment
                    {
                        HunterId = hunter.Id,
                        TargetId = target.Id,
                        GameId = gameId,
                        TargetAssigned = DateTime.UtcNow
                    };

                    await _dbContext.TargetAssignments.AddAsync(assignment);
                }

                game.HasStarted = true;
                game.StartDate = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                // Reload with full graph for mapping.
                Game? reloaded = await GamesWithDetailsTracked().FirstOrDefaultAsync(g => g.Id == gameId);

                resultModel.Data = reloaded!;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to start the game!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to start the game!");
                return resultModel;
            }
        }

        public async Task<ResultModel<int>> CountWinsByUserAsync(Guid userId)
        {
            ResultModel<int> resultModel = new ResultModel<int>();

            try
            {
                resultModel.Data = await _dbContext.Games.CountAsync(g => g.IsFinished && g.WinnerId == userId);
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to count the games!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to count the games!");
                return resultModel;
            }
        }

        #endregion
    }
}
