using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITaskService
    {
        Task<Boolean> CheckingTaskOfUser(int taskId, int studentId);
        Task<IEnumerable<core.Models.Task?>> GetTasksByProjectId(int projectId);
        Task<IEnumerable<core.Models.Task?>> GetTasksByUserId(int userId);
        Task<core.Models.Task?> GetTaskById(int taskId);

        Task<bool> CreateTask(core.Models.Task task);
        Task<bool> UpdateTask(core.Models.Task task);
        Task<bool> DeleteTask(int taskId);
        Task<bool> LoadStatusCompletedTaskByTaskId(int taskId);
    }
}
