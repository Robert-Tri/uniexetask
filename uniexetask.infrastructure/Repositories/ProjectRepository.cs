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
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Project>> GetAllProjects()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Project?> GetProjectByGroupId(int? groupId)
        {
            return await dbSet.FirstOrDefaultAsync(p => p.GroupId == groupId);
        }

        public async Task<Project?> GetProjectPendingByGroupId(int groupId)
        {
            return await dbSet
                .Include(r => r.Topic)
                .FirstOrDefaultAsync(u => u.GroupId == groupId && u.Status.Equals("Status 1"));
        }

        public async Task<Project?> GetProjectsPendingAsync(int projectId)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.ProjectId == projectId && x.Status.Equals("Status 1"));
        }
    }
}
