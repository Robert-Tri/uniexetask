using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<IEnumerable<Student>> GetEligibleStudentsWithUser();
        Task<Student?> GetStudentByUserId(int inviteeId);
        Task<int> GetStudentIdByUserId(int inviteeId);

    }
}
