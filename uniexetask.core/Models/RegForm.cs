using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class RegForm
{
    public int RegFormId { get; set; }

    public int UserRegId { get; set; }

    public int RoleRegId { get; set; }

    public string ContentReg { get; set; } = null!;

    public virtual RoleReg RoleReg { get; set; } = null!;

    public virtual User UserReg { get; set; } = null!;
}
