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
    }
}
