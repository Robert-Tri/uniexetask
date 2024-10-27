using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Funding
{
    public int FundingId { get; set; }

    public int ProjectId { get; set; }

    public double? AmountMoney { get; set; }

    public DateTime ApprovedDate { get; set; }

    public int? DocumentId { get; set; }

    public string Status { get; set; } = null!;

    public virtual Document? Document { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<UsagePlan> UsagePlans { get; set; } = new List<UsagePlan>();
}
