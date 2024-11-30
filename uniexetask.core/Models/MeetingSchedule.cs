using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class MeetingSchedule
{
    public int ScheduleId { get; set; }

    public string MeetingScheduleName { get; set; } = null!;

    public int GroupId { get; set; }

    public int MentorId { get; set; }

    public string Location { get; set; } = null!;

    public DateTime MeetingDate { get; set; }

    public int Duration { get; set; }

    public string Type { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Url { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Mentor Mentor { get; set; } = null!;
}
