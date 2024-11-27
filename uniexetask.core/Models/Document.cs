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

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int UploadBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Funding> Fundings { get; set; } = new List<Funding>();

    public virtual User? ModifiedByNavigation { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual User UploadByNavigation { get; set; } = null!;
}
