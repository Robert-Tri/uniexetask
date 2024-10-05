using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class StudentService : IStudentService
    {
        public IUnitOfWork _unitOfWork;
        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Student>> GetAllStudent()
        {
            var studentList = await _unitOfWork.Students.GetAsync();
            return studentList;
        }
        public async Task<Student?> GetStudentById(int studentId)
        {
            if (studentId > 0)
            {
                var users = await _unitOfWork.Students.GetByIDAsync(studentId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<Student?> GetStudentByCode(string studentCode)
        {
            if (!string.IsNullOrEmpty(studentCode))
            {
                var students = await _unitOfWork.Students.GetAsync();
                var student = students.FirstOrDefault(s => s.StudentCode == studentCode);
                return student;
            }
            return null;
        }

    }
}
