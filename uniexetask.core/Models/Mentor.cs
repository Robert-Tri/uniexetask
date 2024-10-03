using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Mentor
{
    public int MentorId { get; set; }

    public int UserId { get; set; }

    public string Specialty { get; set; } = null!;

    public virtual ICollection<MeetingSchedule> MeetingSchedules { get; set; } = new List<MeetingSchedule>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
