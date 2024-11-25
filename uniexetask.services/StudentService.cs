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

        public async Task<bool> CreateStudent(Student student)
        {
            if (student != null)
            {
                await _unitOfWork.Students.InsertAsync(student);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<Student?> GetStudentById(int studentId)
        {
            if (studentId > 0)
            {
                var student = await _unitOfWork.Students.GetByIDAsync(studentId);
                if (student != null)
                {
                    var user = await _unitOfWork.Users.GetByIDAsync(student.UserId);
                    student.User = user;
                    return student;
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

        public async Task<Student?> GetStudentByUserId(int userId)
        {
            if (userId > 0)
            {
                var students = await _unitOfWork.Students.GetAsync();

                var student = students.FirstOrDefault(s => s.UserId == userId);
                return student;
            }

            return null;
        }
        public async Task<int?> GetStudentIdByUserId(int userId)
        {
            if (userId > 0)
            {
                var students = await _unitOfWork.Students.GetAsync();

                var student = students.FirstOrDefault(s => s.UserId == userId);
                return student.StudentId;
            }

            return null;
        }

        public async Task<IEnumerable<Student>> GetEligibleStudentsWithUser()
        {
            return await _unitOfWork.Students.GetEligibleStudentsWithUser();
        }
    }
}
