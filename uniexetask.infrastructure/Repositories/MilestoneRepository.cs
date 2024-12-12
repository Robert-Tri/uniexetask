using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    class MilestoneRepository : GenericRepository<Milestone>, IMilestoneRepository
    {
        public MilestoneRepository(UniExetaskContext dbContext) : base(dbContext)
        {

        }

        public async Task<Milestone?> GetMileStoneWithCriteria(int id)
        {
            return await dbSet.Include(m => m.Criteria).FirstOrDefaultAsync(m => m.MilestoneId == id);
        }

        public async Task<Milestone?> GetUndeleteMileStoneWithCriteria(int id)
        {
            return await dbSet.Include(m => m.Criteria).FirstOrDefaultAsync(m => m.MilestoneId == id && !m.IsDeleted);
        }

        public async Task<IEnumerable<Milestone>> GetAllUndeleteMileStoneAsync()
        {
            return await dbSet
                .Where(m => !m.IsDeleted)
                .ToListAsync();
        }
        public async Task<int> GetMaxIdAsync()
        {
            return await dbSet.MaxAsync(m => m.MilestoneId);
        }

    }
}
