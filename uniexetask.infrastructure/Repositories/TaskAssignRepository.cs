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
    class TaskAssignRepository : GenericRepository<TaskAssign>, ITaskAssignRepository
    {
        public TaskAssignRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TaskAssign>> GetTaskAssignsByStudentAsync(int studentId)
        {
            return await dbSet.Where(t => t.StudentId == studentId).Include(r => r.Task).ToListAsync();
        }
    }
}