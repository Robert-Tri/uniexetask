using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ConfigSystemService : IConfigSystemService
    {
        public IUnitOfWork _unitOfWork;
        public ConfigSystemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ConfigSystem>> GetConfigSystems()
        {
            return await _unitOfWork.ConfigSystems.GetAsync();
        }
        public async Task<bool> UpdateConfigSystem(int configSystemId, int? number, DateTime? startDate)
        {
            var existedConfigSystem = _unitOfWork.ConfigSystems.GetConfigSystemByID(configSystemId);
            if (existedConfigSystem == null)
                return false;
            if (existedConfigSystem.ConfigId == 2)
                existedConfigSystem.StartDate = startDate;
            else
                existedConfigSystem.Number = number;
            _unitOfWork.ConfigSystems.Update(existedConfigSystem);
            return await _unitOfWork.SaveAsync() > 0;
        }
    }
}
