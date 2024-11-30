using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IConfigSystemService
    {
        Task<IEnumerable<ConfigSystem>> GetConfigSystems();
        Task<bool> UpdateConfigSystem(int configSystemId, int? number, DateTime? startDate);
        Task<ConfigSystem?> GetConfigSystemById(int id);
    }
}
