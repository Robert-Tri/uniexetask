using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface ITaskAssignService
    {
        Task<IEnumerable<TaskAssign?>> GetTaskAssignsByStudent(int studentId);
        Task<bool> CreateTaskAssign(TaskAssign taskAssign);
        Task<bool> UpdateTaskAssign(TaskAssign taskAssign);
        Task<bool> DeleteTaskAssign(int taskAssignId);

    }
}
