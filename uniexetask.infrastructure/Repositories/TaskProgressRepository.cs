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
    class TaskProgressRepository : GenericRepository<TaskProgress>, ITaskProgressRepository
    {
        public TaskProgressRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<TaskProgress> GetTaskProgressByTaskIdAsync(int taskId)
        {
            return await dbSet
                .FirstOrDefaultAsync(t => t.TaskId == taskId && t.IsDeleted == false);
        }

    }
}
