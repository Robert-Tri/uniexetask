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
        Task<bool> CheckDuplicateStudentCode(string studentCode);
        Task<bool> CheckDuplicateStudenCodeForUpdate(int userId, string newStudentCode);
        Task<Student?> GetStudentById(int studentId);
        Task<Student?> GetStudentByCode(string studentCode);
        Task<IEnumerable<Student>> GetEligibleStudentsWithUser();
        Task<Student?> GetStudentByUserId(int userId);
        Task<int?> GetStudentIdByUserId(int userId);
        Task<bool> CreateStudent(Student student);
        Task<Student?> GetUserRoleStudent(int userId);
        Task<bool> UpdateStudent(Student student);
    }
}
