using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class GroupInvite
{
    public int GroupId { get; set; }

    public int NotificationId { get; set; }

    public int InviterId { get; set; }

    public int InviteeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Status { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual Nofitication Notification { get; set; } = null!;
}
