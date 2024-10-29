using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Milestone
{
    public int MilestoneId { get; set; }

    public string MilestoneName { get; set; } = null!;

    public string? Description { get; set; }

    public double Percentage { get; set; }

    public int SubjectId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();

    public virtual ICollection<MemberScore> MemberScores { get; set; } = new List<MemberScore>();

    public virtual Subject Subject { get; set; } = null!;
}
