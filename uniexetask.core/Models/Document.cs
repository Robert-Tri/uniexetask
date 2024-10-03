using System;
using System.Collections.Generic;

namespace uniexetask.core.Models;

public partial class Document
{
    public int DocumentId { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Url { get; set; } = null!;

    public int UploadBy { get; set; }

    public bool IsFinancialReport { get; set; }

    public virtual Project Project { get; set; } = null!;
}
