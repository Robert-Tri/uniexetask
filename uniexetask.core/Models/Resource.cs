using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Resource
{
    public int ResourceId { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int UploadBy { get; set; }

    public virtual Project Project { get; set; } = null!;
}
