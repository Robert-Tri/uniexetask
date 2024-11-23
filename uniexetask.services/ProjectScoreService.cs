using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ProjectScoreService : IProjectScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<double> GetMileStoneScore(int projectId, int mileStoneId)
        {
            var mileStone = await _unitOfWork.Milestones.GetMileStoneWithCriteria(mileStoneId);

            if (mileStone == null || mileStone.Criteria == null || !mileStone.Criteria.Any())
                return 0;

            var criteriaIds = mileStone.Criteria.Select(c => c.CriteriaId).ToList();

            var projectScores = await _unitOfWork.ProjectScores.GetAsync(
                filter: ps => ps.ProjectId == projectId && criteriaIds.Contains(ps.CriteriaId)
            );

            if (projectScores == null || !projectScores.Any())
                return 0;

            double totalScore = 0;

            foreach (var criterion in mileStone.Criteria)
            {
                var projectScore = projectScores.FirstOrDefault(ps => ps.CriteriaId == criterion.CriteriaId);

                if (projectScore != null)
                    totalScore += projectScore.Score * (criterion.Percentage / 100.0);
            }

            return totalScore;
        }


        public async Task<bool> AddProjecScore(List<ProjectScore> projectScores)
        {
            foreach (var projectScore in projectScores)
            {
                await _unitOfWork.ProjectScores.InsertAsync(projectScore);
            }
            var result = _unitOfWork.Save();
            if (result > 0)
                return true;
            return false;
        }
    }
}
