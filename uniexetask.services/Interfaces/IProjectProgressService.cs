using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IProjectProgressService
    {
        Task<ProjectProgress> GetProjectProgressByProjectId(int projectId);
        Task<bool> LoadProgressUpdateProjectProgressByProjectId(int projectId);
        Task<ProjectProgress> CreateProjectProgress(ProjectProgress projectProgress);
    }
}
