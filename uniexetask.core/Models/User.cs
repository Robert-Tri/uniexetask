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

    public int RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Campus Campus { get; set; } = null!;

    public virtual ICollection<ChatGroup> ChatGroupCreatedByNavigations { get; set; } = new List<ChatGroup>();

    public virtual ICollection<ChatGroup> ChatGroupOwners { get; set; } = new List<ChatGroup>();

    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public virtual ICollection<Document> DocumentModifiedByNavigations { get; set; } = new List<Document>();

    public virtual ICollection<Document> DocumentUploadByNavigations { get; set; } = new List<Document>();

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    public virtual ICollection<Notification> NotificationReceivers { get; set; } = new List<Notification>();

    public virtual ICollection<Notification> NotificationSenders { get; set; } = new List<Notification>();

    public virtual ICollection<ProjectScore> ProjectScores { get; set; } = new List<ProjectScore>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<ChatGroup> ChatGroups { get; set; } = new List<ChatGroup>();
}
