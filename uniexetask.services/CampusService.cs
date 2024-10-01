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
    }
}
