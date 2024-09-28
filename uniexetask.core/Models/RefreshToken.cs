using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class RefreshToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public DateTime Revoked { get; set; }

    public bool Status { get; set; }

    public virtual User User { get; set; } = null!;
}
