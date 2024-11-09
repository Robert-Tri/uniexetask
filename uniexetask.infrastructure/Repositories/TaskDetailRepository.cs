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
    class TaskDetailRepository : GenericRepository<TaskDetail>, ITaskDetailRepository
    {
        public TaskDetailRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<TaskDetail>> GetTaskDetailListByTaskIdAsync(int taskId)
        {
            return await dbSet
                .Where(t => t.TaskId == taskId)
                .ToListAsync();
        }
    }
}
