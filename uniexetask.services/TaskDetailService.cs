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
    public class TaskDetailService : ITaskDetailService
    {
        public IUnitOfWork _unitOfWork;
        public TaskDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<TaskDetail?>> GetTaskDetailListByTaskId(int taskId)
        {
            if (taskId > 0)
            {
                var taskDetailList = await _unitOfWork.TaskDetails.GetTaskDetailListByTaskIdAsync(taskId);
                if (taskDetailList != null)
                {
                    return taskDetailList;
                }
            }
            return null;
        }

        public async Task<bool> CreateTaskDetails(TaskDetail taskDetail)
        {
            if (taskDetail != null)
            {
                await _unitOfWork.TaskDetails.InsertAsync(taskDetail);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<bool> UpdateTaskDetails(TaskDetail taskDetail)
        {
            if (taskDetail != null)
            {
                var obj = await _unitOfWork.TaskDetails.GetByIDAsync(taskDetail.TaskDetailId);

                if (obj != null)
                {
                    obj.TaskId = taskDetail.TaskId;
                    obj.TaskDetailName = taskDetail.TaskDetailName;
                    obj.ProgressPercentage = taskDetail.ProgressPercentage;
                    obj.IsCompleted = false;
                    obj.IsDeleted = false;

                    _unitOfWork.TaskDetails.Update(obj);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<bool> DeleteTaskDetailsByTaskId(int taskId)
        {
            if (taskId > 0)
            {
                var taskDetails = await _unitOfWork.TaskDetails.GetTaskDetailsByTaskIdAsync(taskId);
                if (taskDetails != null && taskDetails.Any())
                {
                    foreach (var taskDetail in taskDetails)
                    {
                        _unitOfWork.TaskDetails.Delete(taskDetail);
                    }
                    var result = await _unitOfWork.SaveAsync();

                    return result > 0;
                }
            }
            return false;
        }
    }
}
