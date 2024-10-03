using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;
public partial class SponsorshipDetail
{
    public int SponsorshipDetailId { get; set; }

    public int ProjectId { get; set; }

    public int SponsorId { get; set; }

    public decimal AmountMoney { get; set; }

    public string SponsorshipStatus { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual Sponsor Sponsor { get; set; } = null!;
}
