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

        public async Task<IEnumerable<core.Models.Task?>> GetTasksByProjectId(int projectId)
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
            if (userId <= 0) return null;

            var studentId = await _unitOfWork.Students.GetStudentIdByUserId(userId);
            if (studentId <= 0) return null;

            var groupId = await _unitOfWork.GroupMembers.GetGroupIdByStudentId(studentId);
            if (groupId <= 0) return null;

            var projectId = await _unitOfWork.Projects.GetProjectIdByGroupId(groupId);
            if (projectId <= 0) return null;

            var tasks = await _unitOfWork.Tasks.GetTasksByProjectAsync(projectId);
            if (tasks == null) return null;

            var taskAssigns = await _unitOfWork.TaskAssigns.GetTaskAssignsByStudentAsync(studentId);
            var result = new List<core.Models.Task?>();

            foreach (var task in tasks)
            {
                foreach (var taskAss in taskAssigns)
                {
                    if (task.TaskId == taskAss.TaskId)
                    {
                        result.Add(task);
                    }
                }
            }

            return result;
        }

        public async Task<bool> CreateTask(core.Models.Task task)
        {
            if (task == null || task.StartDate == null || task.EndDate == null)
            {
                return false; // Hoặc throw exception nếu cần
            }

            var project = await _unitOfWork.Projects.GetByIDAsync(task.ProjectId);
            if (project == null)
            {
                return false;
            }

            // Kiểm tra điều kiện ngày
            if (task.EndDate.Date < DateTime.Now.Date || task.StartDate.Date > task.EndDate.Date)
            {
                return false;
            }

            // Xác định trạng thái của task dựa trên ngày
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

            // Thêm task vào database
            await _unitOfWork.Tasks.InsertAsync(task);
            var result = _unitOfWork.Save();

            if (result > 0)
            {
                var progressCreated = await _taskProgressService.CreateTaskProgressByTaskId(task.TaskId);
                return progressCreated;
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
                    // Kiểm tra điều kiện ngày
                    if (task.StartDate.Date > task.EndDate.Date)
                    {
                        return false;
                    }
                    if(task.Status == null)
                    {
                        // Xác định trạng thái của task dựa trên ngày
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
                    }
                    
                    obj.TaskName = task.TaskName;
                    obj.Description = task.Description;
                    obj.StartDate = task.StartDate;
                    obj.EndDate = task.EndDate;
                    obj.Status = task.Status;

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
                    task.IsDeleted = true;
                    _unitOfWork.Tasks.Update(task);

                    var result = _unitOfWork.Save();

                    return result > 0;
                }
            }
            return false;
        }

    }
}
