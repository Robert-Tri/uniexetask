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
    }
}
