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
        public async Task<bool> CheckDuplicateStudentCode(string studentCode)
        {
            var students = await _unitOfWork.Students.GetAsync(filter: s => s.StudentCode == studentCode);
            if (students.Any())
                return true;
            return false;
        }

        public async Task<bool> CheckDuplicateStudenCodeForUpdate(int userId, string newStudentCode)
        {
            var students = await _unitOfWork.Students.GetAsync(filter: s => s.UserId != userId);
            var existedStudent = students.Where(s => s.StudentCode == newStudentCode);
            if(existedStudent.Any())
                return true;
            return false;
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
        public async Task<Student?> GetUserRoleStudent(int userId)
        {
            return (await _unitOfWork.Students.GetAsync(filter: s => s.UserId == userId, includeProperties: "User")).FirstOrDefault();
        }
        public async Task<bool> UpdateStudent(Student student)
        {
            if(student != null)
            {
                var updateStudent = (await _unitOfWork.Students.GetAsync(filter: s => s.UserId == student.UserId)).FirstOrDefault();
                if(updateStudent != null)
                {
                    student.SubjectId = student.SubjectId;
                    student.Major = student.Major;
                    student.LecturerId = student.LecturerId;
                    student.StudentCode = student.StudentCode;
                    _unitOfWork.Students.Update(updateStudent);
                    var result = _unitOfWork.Save();
                    if(result > 0)
                        return true;
                    return false;
                }
            }
            return false;
        }
        public async Task<IEnumerable<Student>> GetStudentWithoutGroup()
        {
            var allStudents = await _unitOfWork.Students.GetAsync(filter: s => s.IsCurrentPeriod == true, includeProperties: "Lecturer.User,User.Campus");
            var groupMembers = await _unitOfWork.GroupMembers.GetAsync();
            var studentsWithoutGroup = allStudents
                .Where(s => !groupMembers.Any(gm => gm.StudentId == s.StudentId))
                .ToList();

            return studentsWithoutGroup;
        }
    }
}
