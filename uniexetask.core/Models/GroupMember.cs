using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class GroupMember
{
    public int GroupId { get; set; }

    public int StudentId { get; set; }

    public string Role { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
