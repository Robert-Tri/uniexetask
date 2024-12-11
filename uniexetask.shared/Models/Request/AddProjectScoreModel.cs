namespace uniexetask.shared.Models.Request
{
    public class AddProjectScoreModel
    {
        public int ProjectScoreId { get; set; }

        public int ProjectId { get; set; } 

        public string Comment { get; set; } = null!;

        public List<ProjectScore> ProjectScores { get; set; } = new List<ProjectScore>();
    }
    public class ProjectScore
    {
        public int CriteriaId { get; set; }
        public double Score { get; set; }
    }
}
