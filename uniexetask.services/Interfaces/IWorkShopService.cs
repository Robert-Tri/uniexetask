using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IWorkShopService
    {
        Task<IEnumerable<Workshop>> GetWorkShops();
        System.Threading.Tasks.Task CreateWorkShop(Workshop workShop);
        System.Threading.Tasks.Task UpdateWorkShop(Workshop workShop);
        Task<bool> DeleteWorkShop(int workShopId);
    }
}
