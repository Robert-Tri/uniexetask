using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class RoleReg
{
    public int RoleRegId { get; set; }

    public int RoleRegName { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<RegForm> RegForms { get; set; } = new List<RegForm>();
}
