using Gotcha2.Core.Data;
using Gotcha2.Core.Entities.Models;
using Gotcha2.Core.Interfaces;
using Gotcha2.Core.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Gotcha2.Core.Services.Repository
{
    public class TargetAssignmentRepoService : ITargetAssignmentRepoService
    {
        private readonly Gotcha2DBContext _dbContext;

        public TargetAssignmentRepoService(Gotcha2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultModel<List<TargetAssignment>>> GetAllAsync()
        {
            ResultModel<List<TargetAssignment>> resultModel = new ResultModel<List<TargetAssignment>>();

            try
            {
                List<TargetAssignment> assignments = await _dbContext.TargetAssignments
                    .Include(ta => ta.Hunter).ThenInclude(p => p.User)
                    .Include(ta => ta.Target).ThenInclude(p => p.User)
                    .ToListAsync();
                resultModel.Data = assignments;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch the target assignments!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to fetch the target assignments!");
                return resultModel;
            }
        }

        public async Task<ResultModel<TargetAssignment>> GetByIdAsync(Guid id)
        {
            ResultModel<TargetAssignment> resultModel = new ResultModel<TargetAssignment>();

            try
            {
                TargetAssignment? assignment = await _dbContext.TargetAssignments
                    .Include(ta => ta.Hunter).ThenInclude(p => p.User)
                    .Include(ta => ta.Target).ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(ta => ta.Id == id);

                if (assignment == null)
                {
                    resultModel.Errors.Add($"No target assignment found with id {id}!");
                    return resultModel;
                }

                resultModel.Data = assignment;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch the target assignment!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to fetch the target assignment!");
                return resultModel;
            }
        }

        public async Task<ResultModel<TargetAssignment?>> GetMyTargetAsync(Guid gameId, Guid userId)
        {
            ResultModel<TargetAssignment?> resultModel = new ResultModel<TargetAssignment?>();

            try
            {
                TargetAssignment? assignment = await _dbContext.TargetAssignments
                    .Include(ta => ta.Hunter).ThenInclude(p => p.User)
                    .Include(ta => ta.Target).ThenInclude(p => p.User)
                    .Where(ta => ta.GameId == gameId
                              && ta.Hunter.UserId == userId
                              && ta.AssignmentFinished == null)
                    .FirstOrDefaultAsync();

                resultModel.Data = assignment;
                return resultModel;
            }
            catch (TimeoutException)
            {
                resultModel.Errors.Add("The server timed out while trying to fetch your target!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("Something went wrong while trying to fetch your target!");
                return resultModel;
            }
        }

        // Writes go through GameRepoService.StartAsync and KillRepoService.RecordKillAsync —
        // these methods stay to satisfy IRepositoryService<TargetAssignment> but are not exercised by controllers.
        public async Task<ResultModel<TargetAssignment>> AddAsync(TargetAssignment entity)
        {
            ResultModel<TargetAssignment> resultModel = new ResultModel<TargetAssignment>();

            try
            {
                await _dbContext.TargetAssignments.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                resultModel.Data = entity;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to add the target assignment!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to add the target assignment!");
                return resultModel;
            }
        }

        public async Task<ResultModel<TargetAssignment>> UpdateAsync(TargetAssignment entity)
        {
            ResultModel<TargetAssignment> resultModel = new ResultModel<TargetAssignment>();

            try
            {
                TargetAssignment? existing = await _dbContext.TargetAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == entity.Id);

                if (existing == null)
                {
                    resultModel.Errors.Add("No target assignment with this id was found!");
                    return resultModel;
                }

                existing.AssignmentFinished = entity.AssignmentFinished;

                await _dbContext.SaveChangesAsync();
                resultModel.Data = existing;
                return resultModel;
            }
            catch (DbUpdateConcurrencyException)
            {
                resultModel.Errors.Add("The target assignment you are trying to update has already been updated!");
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to update the target assignment!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to update the target assignment!");
                return resultModel;
            }
        }

        public async Task<ResultModel<TargetAssignment>> DeleteAsync(Guid id)
        {
            ResultModel<TargetAssignment> resultModel = new ResultModel<TargetAssignment>();

            try
            {
                TargetAssignment? assignment = await _dbContext.TargetAssignments
                    .FirstOrDefaultAsync(ta => ta.Id == id);

                if (assignment == null)
                {
                    resultModel.Errors.Add("No target assignment with this id was found!");
                    return resultModel;
                }

                _dbContext.TargetAssignments.Remove(assignment);
                await _dbContext.SaveChangesAsync();
                resultModel.Data = assignment;
                return resultModel;
            }
            catch (DbUpdateException)
            {
                resultModel.Errors.Add("Something went wrong while trying to delete the target assignment!");
                return resultModel;
            }
            catch (Exception)
            {
                resultModel.Errors.Add("An unexpected error occurred while trying to delete the target assignment!");
                return resultModel;
            }
        }

        public async Task<bool> DoesItExist(Guid id)
        {
            return await _dbContext.TargetAssignments.AnyAsync(ta => ta.Id == id);
        }
    }
}
