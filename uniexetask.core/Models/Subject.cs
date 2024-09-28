using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = null!;

    public string SubjectName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
