using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using System.Threading.Tasks;

namespace uniexetask.services
{
    public class WorkShopService : IWorkShopService
    {
        private readonly IUnitOfWork _unitOfWork;
        public WorkShopService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async System.Threading.Tasks.Task CreateWorkShop(Workshop workShop)
        {
            workShop.Status = "Status 1";
            await _unitOfWork.WorkShops.InsertAsync(workShop);
            _unitOfWork.Save();
            return;
        }

        public void DeleteWorkShop(int workShopId)
        {
            _unitOfWork.WorkShops.Delete(workShopId);
            _unitOfWork.Save();
            return;
        }

        public async Task<IEnumerable<Workshop>> GetWorkShops()
        {
            return await _unitOfWork.WorkShops.GetAsync();
        }

        public async System.Threading.Tasks.Task UpdateWorkShop(Workshop workShop)
        {
            var workShopToUpdate = await _unitOfWork.WorkShops.GetByIDAsync(workShop.WorkshopId);

            if (workShopToUpdate != null)
            {
                workShopToUpdate.Name = workShop.Name;
                workShopToUpdate.Description = workShop.Description;
                workShopToUpdate.StartDate = workShop.StartDate;
                workShopToUpdate.EndDate = workShop.EndDate;
                workShopToUpdate.Location = workShop.Location;
                workShopToUpdate.RegUrl = workShop.RegUrl;
                workShopToUpdate.Status = workShop.Status;

                _unitOfWork.WorkShops.Update(workShopToUpdate);
                await _unitOfWork.SaveAsync();
            }
            else
            {
                throw new Exception("Workshop not found");
            }
        }

    }
}
