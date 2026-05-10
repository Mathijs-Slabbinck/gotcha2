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

        public async Task<ResultModel<TargetAssignment?>> GetMyTargetAsync(Guid gameId, Guid userId)
        {
            ResultModel<TargetAssignment?> resultModel = new ResultModel<TargetAssignment?>();

            try
            {
                TargetAssignment? assignment = await _dbContext.TargetAssignments
                                                                            .AsNoTracking()
                                                                            .Include(ta => ta.Hunter)
                                                                                .ThenInclude(p => p!.User)
                                                                            .Include(ta => ta.Target)
                                                                                .ThenInclude(p => p!.User)
                                                                            .Where(ta => ta.GameId == gameId
                                                                                      && ta.Hunter!.UserId == userId
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

    }
}
