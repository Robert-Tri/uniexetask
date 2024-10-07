using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Password { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Avatar { get; set; }

    public int CampusId { get; set; }

    public bool Status { get; set; }

    public int RoleId { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<ChatGroup> ChatGroupCreatedByNavigations { get; set; } = new List<ChatGroup>();

    public virtual ICollection<ChatGroup> ChatGroupOwners { get; set; } = new List<ChatGroup>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    public virtual ICollection<Nofitication> NofiticationReceivers { get; set; } = new List<Nofitication>();

    public virtual ICollection<Nofitication> NofiticationSenders { get; set; } = new List<Nofitication>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Sponsorship> Sponsorships { get; set; } = new List<Sponsorship>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
