using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uniexetask.services.Interfaces
{
    public interface ITaskService
    {
        Task<IEnumerable<core.Models.Task?>> GetTasksByProject(int projectId);
        Task<IEnumerable<core.Models.Task?>> GetTasksByStudent(int studentId);

    }
}
