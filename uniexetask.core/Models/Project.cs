using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public int GroupId { get; set; }

    public int TopicId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int SubjectId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Funding> Fundings { get; set; } = new List<Funding>();

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<MemberScore> MemberScores { get; set; } = new List<MemberScore>();

    public virtual ICollection<ProjectProgress> ProjectProgresses { get; set; } = new List<ProjectProgress>();

    public virtual ICollection<ProjectScore> ProjectScores { get; set; } = new List<ProjectScore>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual Topic Topic { get; set; } = null!;

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();
}
