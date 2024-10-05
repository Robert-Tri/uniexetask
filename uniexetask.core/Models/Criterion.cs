using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Criterion
{
    public int CriteriaId { get; set; }

    public string CriteriaName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double Percentage { get; set; }

    public string MilestoneName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public virtual ICollection<ScoreCriterion> ScoreCriteria { get; set; } = new List<ScoreCriterion>();
}
