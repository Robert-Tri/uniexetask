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

        public async Task<MilestoneScoreResult> GetMileStoneScore(int projectId, int mileStoneId)
        {
            var mileStone = await _unitOfWork.Milestones.GetMileStoneWithCriteria(mileStoneId);

            if (mileStone == null || mileStone.Criteria == null || !mileStone.Criteria.Any())
                return new MilestoneScoreResult
                {
                    TotalScore = 0,
                    CriterionScores = new List<CriterionScore>()
                };

            var criteriaIds = mileStone.Criteria.Select(c => c.CriteriaId).ToList();

            var projectScores = await _unitOfWork.ProjectScores.GetAsync(
                filter: ps => ps.ProjectId == projectId && criteriaIds.Contains(ps.CriteriaId)
            );

            if (projectScores == null || !projectScores.Any())
                return new MilestoneScoreResult
                {
                    TotalScore = -1,
                    CriterionScores = mileStone.Criteria.Select(c => new CriterionScore
                    {
                        CriteriaId = c.CriteriaId,
                        CriteriaName = c.CriteriaName,
                        Score = -1 
                    }).ToList()
                };

            double totalScore = 0;
            var criterionScores = new List<CriterionScore>();

            foreach (var criterion in mileStone.Criteria)
            {
                var projectScore = projectScores.FirstOrDefault(ps => ps.CriteriaId == criterion.CriteriaId);

                double criterionScore = 0;
                if (projectScore != null)
                {
                    criterionScore = projectScore.Score * (criterion.Percentage / 100.0);
                    totalScore += criterionScore;
                }

                criterionScores.Add(new CriterionScore
                {
                    CriteriaId = criterion.CriteriaId,
                    CriteriaName = criterion.CriteriaName,
                    Score = projectScore?.Score ?? -1
                });
            }

            return new MilestoneScoreResult
            {
                TotalScore = totalScore,
                CriterionScores = criterionScores
            };
        }



        public async Task<bool> SubmitProjecScore(List<ProjectScore> projectScores)
        {
            foreach (var projectScore in projectScores)
            {
                var existedProjectScore =  (await _unitOfWork.ProjectScores.GetAsync(filter: ps => ps.CriteriaId == projectScore.CriteriaId && ps.ProjectId == projectScore.ProjectId)).FirstOrDefault();
                if(existedProjectScore == null) 
                    await _unitOfWork.ProjectScores.InsertAsync(projectScore);
                else
                {
                    existedProjectScore.Score = projectScore.Score;
                    existedProjectScore.Comment = projectScore.Comment;
                    _unitOfWork.ProjectScores.Update(existedProjectScore);
                }
            }
            var result = _unitOfWork.Save();
            if (result > 0)
                return true;
            return false;
        }
    }
    public class MilestoneScoreResult
    {
        public double TotalScore { get; set; }
        public List<CriterionScore> CriterionScores { get; set; }
    }

    public class CriterionScore
    {
        public int CriteriaId { get; set; }
        public string CriteriaName { get; set; }
        public double Score { get; set; }
    }
}
