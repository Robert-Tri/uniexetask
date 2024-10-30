using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uniexetask.services.Interfaces
{
    public interface ITaskProgressService
    {
        Task<bool> CreateTaskProgressByTaskId(int taskId);

    }
}
