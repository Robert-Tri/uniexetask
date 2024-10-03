using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ScoreComponent
{
    public int ComponentId { get; set; }

    public string ComponentName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Percentage { get; set; }

    public string MilestoneName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
