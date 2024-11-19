using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITaskProgressService
    {
        Task<TaskProgress> GetTaskProgressByTaskId(int taskId);
        Task<bool> CreateTaskProgressByTaskId(int taskId);
        Task<bool> LoadProgressUpdateTaskProgressByTaskId(int taskId);
        Task<bool> UpdateTaskProgressByTaskId(TaskProgress taskProgress);

    }
}
