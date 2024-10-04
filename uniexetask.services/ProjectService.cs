using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ProjectService : IProjectService
    {
        public IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Project>> GetAllProjects()

        {
            var project = await _unitOfWork.Projects.GetAllProjects();
            return project;
        }

        public async Task<Project?> GetProjectsPendingAsync(int projectId)
        {
            var project = await _unitOfWork.Projects.GetProjectsPendingAsync(projectId);
            return project;
        }
    }
}
