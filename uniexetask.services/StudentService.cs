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
    }
}
