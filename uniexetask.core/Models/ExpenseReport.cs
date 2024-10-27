using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ExpenseReport
{
    public int ExpenseReportId { get; set; }

    public int UsagePlanId { get; set; }

    public double SpentAmount { get; set; }

    public DateTime SpentDate { get; set; }

    public string ReceiptUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual UsagePlan UsagePlan { get; set; } = null!;
}
