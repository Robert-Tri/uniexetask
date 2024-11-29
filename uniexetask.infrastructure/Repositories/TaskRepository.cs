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
    class TaskRepository : GenericRepository<core.Models.Task>, ITaskRepository
    {
        public TaskRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<core.Models.Task>> GetTasksByProjectAsync(int projectId)
        {
            return await dbSet
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
        }
    }
}
