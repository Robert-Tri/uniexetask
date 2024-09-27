using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Project
{
    public int ProjectId { get; set; }

    public string TopicCode { get; set; } = null!;

    public string TopicName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int SubjectId { get; set; }

    public int Score { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Requirement> Requirements { get; set; } = new List<Requirement>();

    public virtual ICollection<Resource> Resources { get; set; } = new List<Resource>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Label> Labels { get; set; } = new List<Label>();

    public virtual ICollection<Mentor> Mentors { get; set; } = new List<Mentor>();

    public virtual ICollection<Sponsor> Sponsors { get; set; } = new List<Sponsor>();
}
