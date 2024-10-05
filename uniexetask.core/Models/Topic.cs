using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Topic
{
    public int TopicId { get; set; }

    public string TopicCode { get; set; } = null!;

    public string TopicName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
