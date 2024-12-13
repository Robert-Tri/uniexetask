using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.shared.Models.Request
{
    public class MilestoneWithCriteriaModel
    {
        public int MilestoneId { get; set; }

        public string MilestoneName { get; set; } = null!;

        public string? Description { get; set; }

        public double Percentage { get; set; }

        public int SubjectId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public List<Criterion> Criteria { get; set; }

    }
}
