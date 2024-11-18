using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<IEnumerable<Project>> GetAllProjects();
        Task<Project?> GetProjectPendingByGroupId(int groupId);
        Task<Project?> GetProjectsPendingAsync(int projectId);
        Task<Project?> GetProjectByGroupId(int? groupId);
        Task<int> GetProjectIdByGroupId(int groupId);

    }
}
