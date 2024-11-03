using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class TaskService : ITaskService
    {
        public IUnitOfWork _unitOfWork;
        private readonly ITaskProgressService _taskProgressService;

        public TaskService(IUnitOfWork unitOfWork, ITaskProgressService taskProgressService)
        {
            _unitOfWork = unitOfWork;
            _taskProgressService = taskProgressService;
        }

        public async Task<core.Models.Task?> GetTaskById(int taskId)
        {
            if (taskId > 0)
            {
                var task = await _unitOfWork.Tasks.GetByIDAsync(taskId);
                if (task != null) {
                    return task;
                }
            }
            return null;
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

        public async Task<IEnumerable<core.Models.Task?>> GetTasksByUserId(int userId)
        {
            if (userId > 0)
            {
                var student = await _unitOfWork.Students.GetStudentByUserId(userId);
                if (student == null)
                {
                    return null;
                }
                var studentId = student.StudentId;
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

        public async Task<bool> CreateTask(core.Models.Task task)
        {
            if (task != null)
            {
                var project = await _unitOfWork.Projects.GetByIDAsync(task.ProjectId);
                if(project == null)
                {
                    return false;
                }
                DateTime dateee = DateTime.Now;
                if (task.StartDate.Date < DateTime.Now.Date || task.StartDate.Date > task.EndDate.Date)
                {
                    return false;
                }
                else
                {
                    if (task.StartDate.Date > DateTime.Now.Date)
                    {
                        task.Status = nameof(TasksStatus.Not_Started);
                    }
                    else if (task.EndDate.Date > DateTime.Now.Date)
                    {
                        task.Status = nameof(TasksStatus.In_Progress);
                    }
                    else
                    {
                        task.Status = nameof(TasksStatus.Overdue);
                    }
                    await _unitOfWork.Tasks.InsertAsync(task);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                    {
                        var progressCreated = await _taskProgressService.CreateTaskProgressByTaskId(task.TaskId);
                        return progressCreated;
                    }
                }
            }
            return false;
        }

        public async Task<bool> UpdateTask(core.Models.Task task)
        {
            if (task != null)
            {
                var obj = await _unitOfWork.Tasks.GetByIDAsync(task.TaskId);

                if (obj != null)
                {
                    obj.TaskName = task.TaskName;
                    obj.Description = task.Description;
                    obj.StartDate = task.StartDate;
                    obj.EndDate = task.EndDate;

                    _unitOfWork.Tasks.Update(obj);

                    var result = _unitOfWork.Save();
                    return result > 0;
                }
            }
            return false;
        }

        public async Task<bool> DeleteTask(int taskId)
        {
            if (taskId > 0)
            {
                var task = await _unitOfWork.Tasks.GetByIDAsync(taskId);
                if (task != null)
                {
                    _unitOfWork.Tasks.Delete(task);
                    var result = await _unitOfWork.SaveAsync();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

    }
}
