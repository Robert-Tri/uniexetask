using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Score
{
    public int ScoreId { get; set; }

    public int ComponentId { get; set; }

    public int ProjectId { get; set; }

    public double Score1 { get; set; }

    public string ScoredBy { get; set; } = null!;

    public bool RatingStatus { get; set; }

    public virtual ScoreComponent Component { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
