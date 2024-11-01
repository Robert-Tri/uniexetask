namespace uniexetask.api.Models.Request
{
    public class AddMemberScoreModel
    {
        public int MemberScoreId { get; set; }

        public int StudentId { get; set; }

        public int ProjectId { get; set; }

        public int MilestoneId { get; set; }

        public double Score { get; set; }

        public string? Comment { get; set; }

        public int ScoredBy { get; set; }

        public DateTime ScoringDate { get; set; }
    }
}
