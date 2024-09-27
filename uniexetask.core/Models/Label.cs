using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Label
{
    public int LabelId { get; set; }

    public string LabelName { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
