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
    public class TaskProgressService : ITaskProgressService
    {
        public IUnitOfWork _unitOfWork;

        public TaskProgressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TaskProgress> GetTaskProgressByTaskId(int taskId)
        {
            if (taskId > 0)
            {
                var progress = await _unitOfWork.TaskProgresses.GetTaskProgressByTaskIdAsync(taskId);

                if (progress != null)
                {
                    return progress;
                }
            }

            return null;
        }

        public async Task<bool> CreateTaskProgressByTaskId(int taskId)
        {
            if (taskId > 0)
            {
                var task = await _unitOfWork.Tasks.GetByIDAsync(taskId);
                if (task == null)
                {
                    return false;
                }
                // Tạo TaskProgress
                TaskProgress progress = new TaskProgress
                {
                    TaskId = taskId,
                    ProgressPercentage = 0,
                    UpdatedDate = DateTime.Now,
                    IsDeleted = false
                };

                await _unitOfWork.TaskProgresses.InsertAsync(progress);
                var result = _unitOfWork.Save();

                return result > 0;
            }
            return false;
        }

        public async Task<bool> UpdateTaskProgressByTaskId(TaskProgress taskProgress)
        {
            if (taskProgress != null)
            {
                var taskProgressPresent = await _unitOfWork.TaskProgresses.GetTaskProgressByTaskIdAsync(taskProgress.TaskId);
                if (taskProgressPresent != null)
                {
                    taskProgressPresent.IsDeleted = true;

                    _unitOfWork.TaskProgresses.Update(taskProgressPresent);

                    _unitOfWork.Save();
                }
                // Tạo TaskProgress
                TaskProgress progress = new TaskProgress
                {
                    TaskId = taskProgress.TaskProgressId,
                    ProgressPercentage = taskProgress.ProgressPercentage,
                    UpdatedDate = DateTime.Now,
                    Note = taskProgress.Note,
                    IsDeleted = false
                };

                await _unitOfWork.TaskProgresses.InsertAsync(progress);
                var result = _unitOfWork.Save();

                return result > 0;
            }
            return false;
        }

        public async Task<bool> LoadProgressUpdateTaskProgressByTaskId(int taskId)
        {
            var taskProgressPresent = await _unitOfWork.TaskProgresses.GetTaskProgressByTaskIdAsync(taskId);
            if (taskProgressPresent != null)
            {
                taskProgressPresent.IsDeleted = true;

                _unitOfWork.TaskProgresses.Update(taskProgressPresent);

                _unitOfWork.Save();
            }
            decimal totalProgressPercentage = 0;
            var taskDetails = await _unitOfWork.TaskDetails.GetTaskDetailsByTaskIdAsync(taskId);
            foreach (var item in taskDetails)
            {
                if (item.IsCompleted)
                {
                    totalProgressPercentage += item.ProgressPercentage;
                }
            }
            // Tạo TaskProgress
            TaskProgress progress = new TaskProgress
            {
                TaskId = taskId,
                ProgressPercentage = totalProgressPercentage,
                UpdatedDate = DateTime.Now,
                Note = taskProgressPresent != null ? taskProgressPresent.Note : null,
                IsDeleted = false
            };

            await _unitOfWork.TaskProgresses.InsertAsync(progress);
            var result = _unitOfWork.Save();

            return result > 0;
        }

    }
}
