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

        public async Task<ProjectScoreResult> GetTotalProjectScore(int projectId)
        {
            var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
            var group = await _unitOfWork.Groups.GetByIDAsync(project.GroupId);

            var milestones = await _unitOfWork.Milestones.GetAsync(filter: m => m.SubjectId == group.SubjectId);

            if (milestones == null || !milestones.Any())
            {
                return new ProjectScoreResult
                {
                    TotalScore = 0,
                    MilestoneScores = new List<MilestoneScoreResult>()
                };
            }

            double totalProjectScore = 0;
            var milestoneScores = new List<MilestoneScoreResult>();

            foreach (var milestone in milestones)
            {
                var mileStoneSelected = await _unitOfWork.Milestones.GetMileStoneWithCriteria(milestone.MilestoneId);
                if (mileStoneSelected == null || mileStoneSelected.Criteria == null || !mileStoneSelected.Criteria.Any())
                {
                    continue;
                }

                var criteriaIds = mileStoneSelected.Criteria.Select(c => c.CriteriaId).ToList();
                var projectScores = await _unitOfWork.ProjectScores.GetAsync(
                    filter: ps => ps.ProjectId == projectId && criteriaIds.Contains(ps.CriteriaId)
                );
                if (projectScores == null || !projectScores.Any())
                {
                    milestoneScores.Add(new MilestoneScoreResult
                    {
                        MilestoneId = milestone.MilestoneId,
                        TotalScore = -1
                    });
                    continue;
                }

                double totalMilestoneScore = 0;

                foreach (var criterion in mileStoneSelected.Criteria)
                {
                    var projectScore = projectScores.FirstOrDefault(ps => ps.CriteriaId == criterion.CriteriaId);
                    double criterionScore = 0;

                    if (projectScore != null)
                    {
                        criterionScore = projectScore.Score * (criterion.Percentage / 100.0);
                        totalMilestoneScore += criterionScore;
                    }
                }

                milestoneScores.Add(new MilestoneScoreResult
                {
                    MilestoneId = milestone.MilestoneId,
                    TotalScore = totalMilestoneScore
                });

                if (totalMilestoneScore != 0)
                {
                    totalProjectScore += (totalMilestoneScore * milestone.Percentage / 100.0);
                }
            }
            return new ProjectScoreResult
            {
                TotalScore = Math.Round(totalProjectScore, 2),
                MilestoneScores = milestoneScores
            };
        }


        public async Task<ProjectScoreResult> GetTotalProjectScoreV2(int projectId)
        {
            var allProjectScores = await _unitOfWork.ProjectScores.GetAsync(ps => ps.ProjectId == projectId);

            var criteriaIds = allProjectScores.Select(ps => ps.CriteriaId).Distinct().ToList();
            var criteria = await _unitOfWork.Criterion.GetAsync(c => criteriaIds.Contains(c.CriteriaId));

            var milestoneIds = criteria.Select(c => c.MilestoneId).Distinct().ToList();
            var milestones = await _unitOfWork.Milestones.GetAsync(m => milestoneIds.Contains(m.MilestoneId));

            List<Milestone> milestoneList = new List<Milestone>();
            foreach (var item in milestoneIds)
            {
                var milestone = await _unitOfWork.Milestones.GetByIDAsync(item);
                milestone.MemberScores = null;
                milestoneList.Add(milestone);
            }

            var milestoneScores = milestones.Select(milestone => {
                var relatedCriteria = criteria.Where(c => c.MilestoneId == milestone.MilestoneId).ToList();
                var relatedScores = allProjectScores.Where(ps => relatedCriteria.Any(c => c.CriteriaId == ps.CriteriaId)).ToList();

                double totalMilestoneScore = 0;
                foreach (var criterion in relatedCriteria)
                {
                    var projectScore = relatedScores.FirstOrDefault(ps => ps.CriteriaId == criterion.CriteriaId);
                    if (projectScore != null)
                    {
                        totalMilestoneScore += projectScore.Score * (criterion.Percentage / 100.0);
                    }
                }

                return new MilestoneScoreResult
                {
                    MilestoneId = milestone.MilestoneId,
                    Percentage = milestone.Percentage,
                    TotalScore = relatedCriteria.Any() ? Math.Round(totalMilestoneScore, 2) : -1, // Nếu không có điểm, trả về -1
                    CriterionScores = relatedScores.Select(ps => new CriterionScore
                    {
                        CriteriaId = ps.CriteriaId,
                        CriteriaName = criteria.First(c => c.CriteriaId == ps.CriteriaId).CriteriaName,
                        Score = ps.Score
                    }).ToList()
                };
            }).ToList();

            // Tính tổng điểm của toàn bộ project (bỏ qua milestones không có điểm)
            double totalProjectScore = 0;
            foreach (var milestone in milestoneScores)
            {
                if (milestone.TotalScore != -1)
                {
                    var milestoneEntity = milestones.First(m => m.MilestoneId == milestone.MilestoneId);
                    totalProjectScore += milestone.TotalScore * (milestoneEntity.Percentage / 100.0);
                }
            }

            return new ProjectScoreResult
            {
                TotalScore = Math.Round(totalProjectScore, 2),
                MilestoneScores = milestoneScores,
                MilestoneIds = milestoneList
            };
        }
    }

    public class ProjectScoreResult
    {
        public double TotalScore { get; set; }  
        public List<MilestoneScoreResult> MilestoneScores { get; set; }
        public List<Milestone>? MilestoneIds { get; set; }

    }

    public class MilestoneScoreResult
    {
        public int MilestoneId { get; set; }
        public double Percentage { get; set; }
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
