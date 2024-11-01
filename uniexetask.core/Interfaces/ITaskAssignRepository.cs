using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface ITaskAssignRepository : IGenericRepository<TaskAssign>
    {
        Task<IEnumerable<TaskAssign>> GetTaskAssignsByStudentAsync(int studentId);
        Task<IEnumerable<TaskAssign>> GetTaskAssignsByTaskIdAsync(int taskId);

    }
}
