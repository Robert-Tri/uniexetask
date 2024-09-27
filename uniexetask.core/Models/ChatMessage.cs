using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ChatMessage
{
    public int MessageId { get; set; }

    public int ChatGroupId { get; set; }

    public int UserId { get; set; }

    public string MessageContent { get; set; } = null!;

    public DateTime SendDatetime { get; set; }

    public virtual ChatGroup ChatGroup { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
