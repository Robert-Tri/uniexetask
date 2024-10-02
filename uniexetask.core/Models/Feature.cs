using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Feature
{
    public int FeatureId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
