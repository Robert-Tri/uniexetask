using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Campus
{
    public int CampusId { get; set; }

    public string CampusCode { get; set; } = null!;

    public string CampusName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
