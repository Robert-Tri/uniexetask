using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public int ProjectId { get; set; }

    public string GroupName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<GroupInvite> GroupInvites { get; set; } = new List<GroupInvite>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<MeetingSchedule> MeetingSchedules { get; set; } = new List<MeetingSchedule>();

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();
}
