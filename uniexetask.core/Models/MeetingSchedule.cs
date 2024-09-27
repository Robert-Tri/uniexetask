using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class MeetingSchedule
{
    public int ScheduleId { get; set; }

    public int GroupId { get; set; }

    public int MentorId { get; set; }

    public int Location { get; set; }

    public DateTime MeetingDate { get; set; }

    public int Duration { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual Mentor Mentor { get; set; } = null!;
}
