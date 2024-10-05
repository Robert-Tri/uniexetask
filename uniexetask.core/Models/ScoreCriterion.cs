using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ScoreCriterion
{
    public int ScoreId { get; set; }

    public int CriteriaId { get; set; }

    public int ProjectId { get; set; }

    public double Score { get; set; }

    public string ScoredBy { get; set; } = null!;

    public string Comment { get; set; } = null!;

    public bool RatingStatus { get; set; }

    public virtual Criterion Criteria { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
