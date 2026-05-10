using Gotcha2.Core.Data;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.Core.Services.Repository
{
    public class KillRepoService : IKillRepoService
    {
        private readonly Gotcha2DBContext _dbContext;

        public KillRepoService(Gotcha2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region === READS ===

        public async Task<ResultModel<List<Kill>>> GetByGameAsync(Guid gameId)
        {
            ResultModel<List<Kill>> resultModel = new ResultModel<List<Kill>>();

            try
            {
                List<Kill> kills = await _dbContext.Kills
                                                        .AsNoTracking()
                                                        .Include(k => k.Killer)
                                                            .ThenInclude(p => p!.User)
                                                        .Include(k => k.Victim)
                                                            .ThenInclude(p => p!.User)
                                                        .Where(k => k.GameId == gameId)
                                                        .OrderByDescending(k => k.Moment)
                                                        .ToListAsync();
                resultModel.Data = kills;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch the kills!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to fetch the kills!");
                return resultModel;
            }
        }

        #endregion

        #region === KILL RESOLUTION ===

        public async Task<ResultModel<Kill>> RecordKillAsync(Guid gameId, Guid victimPlayerId, Guid actingUserId)
        {
            ResultModel<Kill> resultModel = new ResultModel<Kill>();

            try
            {
                // 1. Load the game.
                Game? game = await _dbContext.Games
                                                .Include(g => g.Players)
                                                    .ThenInclude(p => p.User)
                                                .FirstOrDefaultAsync(g => g.Id == gameId);

                if (game == null)
                {
                    resultModel.Errors.Add("No game with this id was found!");
                    return resultModel;
                }

                if (!game.HasStarted)
                {
                    resultModel.Errors.Add("Game has not started yet.");
                    return resultModel;
                }

                if (game.IsFinished)
                {
                    resultModel.Errors.Add("Game has already finished.");
                    return resultModel;
                }

                // 2. Validate the victim.
                Player? victim = game.Players.FirstOrDefault(p => p.Id == victimPlayerId);

                if (victim == null)
                {
                    resultModel.Errors.Add("Victim is not a player in this game.");
                    return resultModel;
                }

                if (!victim.IsAlive)
                {
                    resultModel.Errors.Add("Victim is already dead.");
                    return resultModel;
                }

                // 3. Load this game's open assignments once, then pick out the two we need.
                List<TargetAssignment> openAssignments = await _dbContext.TargetAssignments
                                                .Include(ta => ta.Hunter)
                                                    .ThenInclude(p => p!.User)
                                                .Where(ta => ta.GameId == gameId && ta.AssignmentFinished == null)
                                                .ToListAsync();

                // Hunter->victim assignment: its Hunter is the killer.
                TargetAssignment? hunterAssignment = openAssignments
                                                .FirstOrDefault(ta => ta.TargetId == victimPlayerId);

                if (hunterAssignment == null)
                {
                    resultModel.Errors.Add("No open target assignment found for this victim.");
                    return resultModel;
                }

                Player killer = hunterAssignment.Hunter!;

                // 4. Authorise: actingUserId must be the killer's user OR the victim's user.
                bool isKiller = killer.UserId == actingUserId;
                bool isVictim = victim.UserId == actingUserId;

                if (!isKiller && !isVictim)
                {
                    resultModel.Errors.Add("Only the assigned hunter or the victim can record this kill.");
                    return resultModel;
                }

                // 5. Victim's own open assignment — its Target is what the killer inherits.
                TargetAssignment? victimAssignment = openAssignments
                                                .FirstOrDefault(ta => ta.HunterId == victimPlayerId);

                Guid? inheritedTargetPlayerId = victimAssignment?.TargetId;

                // Special case: when only killer + victim remain alive, killer's new target is themselves
                // in the ring, which means the game ends. Skip creating a follow-up assignment in that case.
                bool inheritedSelfTarget = inheritedTargetPlayerId == killer.Id;

                // 6. Create the Kill row.
                Kill newKill = new Kill
                {
                    GameId = gameId,
                    KillerId = killer.Id,
                    Killer = killer,
                    VictimId = victim.Id,
                    Victim = victim,
                    Moment = DateTime.UtcNow
                };

                await _dbContext.Kills.AddAsync(newKill);

                // 7. Close hunter->victim assignment + link to the new Kill.
                hunterAssignment.AssignmentFinished = DateTime.UtcNow;
                hunterAssignment.Kill = newKill;

                // 8. Close victim's own open assignment (no kill linkage — victim never killed their target).
                if (victimAssignment != null)
                {
                    victimAssignment.AssignmentFinished = DateTime.UtcNow;
                }

                // 9. Mark victim dead.
                victim.IsAlive = false;

                // 10. Issue the killer's new assignment, if there is a target to inherit and it's not the killer themselves.
                if (inheritedTargetPlayerId.HasValue && !inheritedSelfTarget)
                {
                    TargetAssignment followUp = new TargetAssignment
                    {
                        HunterId = killer.Id,
                        TargetId = inheritedTargetPlayerId.Value,
                        GameId = gameId,
                        TargetAssigned = DateTime.UtcNow
                    };

                    await _dbContext.TargetAssignments.AddAsync(followUp);
                }

                // 11. End game if <= 1 alive.
                int aliveCount = game.Players.Count(p => p.IsAlive);

                if (aliveCount <= 1)
                {
                    Player? lastAlive = game.Players.FirstOrDefault(p => p.IsAlive);

                    game.IsFinished = true;
                    game.EndDate = DateTime.UtcNow;
                    game.WinnerId = lastAlive?.UserId;
                }

                // 12. Save it all.
                await _dbContext.SaveChangesAsync();

                resultModel.Data = newKill;
                return resultModel;
            }
            catch (DbUpdateConcurrencyException)
            {
                resultModel.Errors.Add("Someone else updated the game state — refresh and try again.");
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to record the kill!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to record the kill!");
                return resultModel;
            }
        }

        public async Task<ResultModel<int>> CountByKillerUserAsync(Guid userId)
        {
            ResultModel<int> resultModel = new ResultModel<int>();

            try
            {
                resultModel.Data = await _dbContext.Kills.CountAsync(k => k.Killer!.UserId == userId);
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to count the kills!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to count the kills!");
                return resultModel;
            }
        }

        #endregion
    }
}
