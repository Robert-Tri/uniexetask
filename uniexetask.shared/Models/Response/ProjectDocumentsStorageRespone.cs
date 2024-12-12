using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.shared.Models.Response
{
    public class ProjectDocumentsStorageRespone
    {
        public Project Project { get; set; }
        public long TotalStorage { get; set; }
    }
}
