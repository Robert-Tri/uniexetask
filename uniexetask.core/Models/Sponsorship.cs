using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Sponsorship
{
    public int SponsorshipDetailId { get; set; }

    public int ProjectId { get; set; }

    public int UserId { get; set; }

    public decimal AmountMoney { get; set; }

    public bool IsInvesting { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
