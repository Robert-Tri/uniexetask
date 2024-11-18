using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITaskDetailService
    {
        Task<IEnumerable<TaskDetail?>> GetTaskDetailListByTaskId(int taskId);
        Task<TaskDetail?> GetTaskDetailById(int taskDetailId);
        Task<bool> CreateTaskDetails(TaskDetail taskDetail);
        Task<bool> UpdateTaskDetails(TaskDetail taskDetail);
        Task<bool> DeleteTaskDetailsByTaskId(int taskId);
        Task<bool> DeleteTaskDetail(int taskDetailId);

    }
}
