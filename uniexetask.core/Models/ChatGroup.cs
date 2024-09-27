using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ChatGroup
{
    public int ChatGroupId { get; set; }

    public string ChatboxName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }

    public int OwnerId { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User Owner { get; set; } = null!;
}
