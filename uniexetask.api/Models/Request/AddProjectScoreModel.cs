namespace uniexetask.api.Models.Request
{
    public class AddProjectScoreModel
    {
        public int ProjectScoreId { get; set; }

        public int CriteriaId { get; set; }

        public int ProjectId { get; set; }

        public double Score { get; set; }

        public string Comment { get; set; } = null!;

        public int ScoredBy { get; set; }

        public DateTime ScoringDate { get; set; }
    }
}
