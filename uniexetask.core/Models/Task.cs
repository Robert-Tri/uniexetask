using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int ProjectId { get; set; }

    public string TaskName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<TaskAssign> TaskAssigns { get; set; } = new List<TaskAssign>();
}
