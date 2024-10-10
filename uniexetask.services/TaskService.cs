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
    public class TaskService : ITaskService
    {
        public IUnitOfWork _unitOfWork;
        public TaskService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<core.Models.Task?>> GetTasksByProject(int projectId)
        {
            if (projectId > 0)
            {
                var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(projectId);
                if (tasks != null)
                {
                    return tasks;
                }
            }
            return null;
        }

    }
}
