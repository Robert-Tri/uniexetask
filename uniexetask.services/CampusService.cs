using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class CampusService : ICampusService
    {
        public IUnitOfWork _unitOfWork;
        public CampusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Campus>> GetAllCampus()
        {
            var campusesList = await _unitOfWork.Campus.GetAsync();
            return campusesList;
        }

        public async Task<Campus?> GetCampusByStudentCode(string studentCode)
        {
            var student = (await _unitOfWork.Students.GetAsync(
                filter: s => s.StudentCode == studentCode && s.IsCurrentPeriod == true,
                includeProperties: "User.Campus")).FirstOrDefault(); 

            if (student == null)
            {
                return null;
            }

            return student.User.Campus;
        }


    }
}
