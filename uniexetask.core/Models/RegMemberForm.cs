using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class RegMemberForm
{
    public int RegMemberId { get; set; }

    public int GroupId { get; set; }

    public string Description { get; set; } = null!;

    public bool Status { get; set; }

    public virtual Group Group { get; set; } = null!;
}
