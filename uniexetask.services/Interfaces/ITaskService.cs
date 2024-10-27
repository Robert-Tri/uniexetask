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
        Task<IEnumerable<core.Models.Task?>> GetTasksByProject(int projectId);
        Task<IEnumerable<core.Models.Task?>> GetTasksByStudent(int studentId);
        Task<bool> CreateTask(core.Models.Task task);
        Task<bool> UpdateTask(core.Models.Task task);
        Task<bool> DeleteTask(int taskId);
    }
}
