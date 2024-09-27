using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Requirement
{
    public int RequirementId { get; set; }

    public int ProjectId { get; set; }

    public int RequestorId { get; set; }

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
