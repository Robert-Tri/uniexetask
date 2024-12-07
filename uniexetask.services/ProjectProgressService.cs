using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using uniexetask.core.Models.Enums;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ProjectProgressService : IProjectProgressService
    {
        public IUnitOfWork _unitOfWork;

        public ProjectProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ProjectProgress> GetProjectProgressByProjectId(int projectId)
        {
            if (projectId > 0)
            {
                var projectProgress = await _unitOfWork.ProjectProgresses.GetProjectProgressByProjectId(projectId);
                if (projectProgress != null)
                {
                    return projectProgress;
                }
            }
            return null;
        }

        public async Task<ProjectProgress> CreateProjectProgress(ProjectProgress projectProgress)
        {
            if (projectProgress != null)
            {
                await _unitOfWork.ProjectProgresses.InsertAsync(projectProgress);

                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                    return projectProgress;
                else
                    return null;
            }
            return null;
        }

        public async Task<bool> LoadProgressUpdateProjectProgressByProjectId(int projectId)
        {
            var taskList = await _unitOfWork.Tasks.GetTasksByProjectAsync(projectId);
            int countCompleted = 0;
            int totalTasks = 0;
            foreach (var task in taskList)
            {
                if(task.Status == nameof(TasksStatus.Completed))
                {
                    countCompleted++;
                }
                totalTasks++;
            }
            var progress = countCompleted * 100 / totalTasks;

            var project = await _unitOfWork.ProjectProgresses.GetProjectProgressByProjectId(projectId);
            project.IsDeleted = true;
            _unitOfWork.ProjectProgresses.Update(project);
            var result = _unitOfWork.Save();
            if (result > 0)
            {
                // Tạo TaskProgress mới
                ProjectProgress proProgress = new ProjectProgress
                {
                    ProjectId = projectId,
                    ProgressPercentage = progress,
                    UpdatedDate = DateTime.Now,
                    Note = project.Note,
                    IsDeleted = false
                };

                await _unitOfWork.ProjectProgresses.InsertAsync(proProgress);
                result = _unitOfWork.Save();
            }

            return result > 0;
        }
    }
}
