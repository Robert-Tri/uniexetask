using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class MemberScore
{
    public int MemberScoreId { get; set; }

    public int StudentId { get; set; }

    public int ProjectId { get; set; }

    public int MilestoneId { get; set; }

    public double Score { get; set; }

    public string? Comment { get; set; }

    public int ScoredBy { get; set; }

    public DateTime ScoringDate { get; set; }

    public virtual Milestone Milestone { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual Mentor ScoredByNavigation { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
