using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Timeline
{
    public int TimelineId { get; set; }

    public string TimelineName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
