using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class TaskProgress
{
    public int TaskProgressId { get; set; }

    public int TaskId { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime UpdatedDate { get; set; }

    public string? Note { get; set; }

    public virtual Task Task { get; set; } = null!;
}
