using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IMilestoneRepository : IGenericRepository<Milestone>
    {
        Task<Milestone?> GetMileStoneWithCriteria(int id);
        Task<Milestone?> GetUndeleteMileStoneWithCriteria(int id);
        Task<IEnumerable<Milestone>> GetAllUndeleteMileStoneAsync();
        Task<int> GetMaxIdAsync();

    }
}
