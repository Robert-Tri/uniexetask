using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ProjectProgress
{
    public int ProjectProgressId { get; set; }

    public int ProjectId { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string? Note { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Project Project { get; set; } = null!;
}
