using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectCode { get; set; } = null!;

    public DateTime SubjectName { get; set; }

    public int Description { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
