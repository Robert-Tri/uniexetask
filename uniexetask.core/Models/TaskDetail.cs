using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class TaskDetail
{
    public int TaskDetailId { get; set; }

    public int TaskId { get; set; }

    public string TaskDetailName { get; set; } = null!;

    public decimal ProgressPercentage { get; set; }

    public bool IsCompleted { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Task Task { get; set; } = null!;
}
