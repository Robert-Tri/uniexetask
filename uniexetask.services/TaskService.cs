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

        public async Task<IEnumerable<core.Models.Task?>> GetTasksByStudent(int studentId)
        {
            if (studentId > 0)
            {
                var groupId = await _unitOfWork.GroupMembers.GetGroupIdByStudentId(studentId);

                if (groupId == null)
                {
                    return null;
                }

                var project = await _unitOfWork.Projects.GetProjectByGroupId(groupId);

                if (project == null)
                {
                    return null;
                }

                var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(project.ProjectId);

                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        task.Project = null;
                    }
                    return tasks;
                }
            }

            return null;
        }

    }
}
