using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Sponsor
{
    public int SponsorId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<SponsorshipDetail> SponsorshipDetails { get; set; } = new List<SponsorshipDetail>();

    public virtual User User { get; set; } = null!;
}
