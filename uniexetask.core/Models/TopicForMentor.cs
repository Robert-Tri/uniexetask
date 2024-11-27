using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class TopicForMentor
{
    public int TopicForMentorId { get; set; }

    public int MentorId { get; set; }

    public string TopicCode { get; set; } = null!;

    public string TopicName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public bool IsRegistered { get; set; }

    public virtual Mentor Mentor { get; set; } = null!;
}
