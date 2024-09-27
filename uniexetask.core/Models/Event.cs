using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Location { get; set; } = null!;

    public string RegUrl { get; set; } = null!;

    public string Status { get; set; } = null!;
}
