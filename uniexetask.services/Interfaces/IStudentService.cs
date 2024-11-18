using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudent();
        Task<Student?> GetStudentById(int studentId);
        Task<Student?> GetStudentByCode(string studentCode);
        Task<IEnumerable<Student>> GetEligibleStudentsWithUser();
        Task<Student?> GetStudentByUserId(int userId);
        Task<int?> GetStudentIdByUserId(int userId);

    }
}
