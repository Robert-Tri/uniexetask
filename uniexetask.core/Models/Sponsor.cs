using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Sponsor
{
    public int SponsorId { get; set; }

    public int UserId { get; set; }

    public string Type { get; set; } = null!;

    public string InvestmentField { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
