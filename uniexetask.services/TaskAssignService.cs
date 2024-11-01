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
    public class TaskAssignService : ITaskAssignService
    {
        public IUnitOfWork _unitOfWork;
        public TaskAssignService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<core.Models.TaskAssign?>> GetTaskAssignsByStudent(int studentId)
        {
            if (studentId > 0)
            {
                var tasks = await _unitOfWork.TaskAssigns.GetTaskAssignsByStudentAsync(studentId);
                if (tasks != null)
                {
                    return tasks;
                }
            }
            return null;
        }

        public async Task<bool> CreateTaskAssign(TaskAssign taskAssign)
        {
            if (taskAssign != null)
            {
                await _unitOfWork.TaskAssigns.InsertAsync(taskAssign);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<bool> UpdateTaskAssign(TaskAssign taskAssign)
        {
            if (taskAssign != null)
            {
                var obj = await _unitOfWork.TaskAssigns.GetByIDAsync(taskAssign.TaskAssignId);

                if (obj != null)
                {
                    obj.TaskId = taskAssign.TaskId;
                    obj.StudentId = taskAssign.StudentId;
                    obj.AssignedDate = DateTime.Now;

                    _unitOfWork.TaskAssigns.Update(obj);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<bool> DeleteTaskAssignByTaskId(int taskId)
        {
            if (taskId > 0)
            {
                var tasks = await _unitOfWork.TaskAssigns.GetTaskAssignsByTaskIdAsync(taskId);
                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        _unitOfWork.Tasks.Delete(task);
                    }
                    var result = await _unitOfWork.SaveAsync();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<TaskAssign?>> GetTaskAssignsByTaskId(int taskId)
        {
            if (taskId > 0)
            {
                var tasks = await _unitOfWork.TaskAssigns.GetTaskAssignsByTaskIdAsync(taskId);
                if (tasks != null)
                {
                    return tasks;
                }
            }
            return null;
        }
    }
}
