using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public int SubjectId { get; set; }

    public bool HasMentor { get; set; }

    public bool IsCurrentPeriod { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<GroupInvite> GroupInvites { get; set; } = new List<GroupInvite>();

    public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    public virtual ICollection<MeetingSchedule> MeetingSchedules { get; set; } = new List<MeetingSchedule>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<RegMemberForm> RegMemberForms { get; set; } = new List<RegMemberForm>();

    public virtual ICollection<RegTopicForm> RegTopicForms { get; set; } = new List<RegTopicForm>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();
}
