using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
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
    }
}
