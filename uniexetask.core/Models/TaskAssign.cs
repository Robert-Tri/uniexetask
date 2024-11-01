using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class TaskAssign
{
    public int TaskAssignId { get; set; }

    public int TaskId { get; set; }

    public int StudentId { get; set; }

    public DateTime AssignedDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Task Task { get; set; } = null!;
}
