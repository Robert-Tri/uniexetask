using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class RegTopicForm
{
    public int RegId { get; set; }

    public int GroupId { get; set; }

    public string TopicCode { get; set; } = null!;

    public string TopicName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool Status { get; set; }

    public virtual Group Group { get; set; } = null!;
}
