using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class ConfigSystem
{
    public int ConfigId { get; set; }

    public string ConfigName { get; set; } = null!;

    public int? Number { get; set; }

    public DateTime? StartDate { get; set; }
}
