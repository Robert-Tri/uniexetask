using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class UsagePlan
{
    public int UsagePlanId { get; set; }

    public int FundingId { get; set; }

    public string Title { get; set; } = null!;

    public double Amount { get; set; }

    public string Description { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<ExpenseReport> ExpenseReports { get; set; } = new List<ExpenseReport>();

    public virtual Funding Funding { get; set; } = null!;
}
