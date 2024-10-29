using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class MilestoneService : IMilestoneService
    {
        public IUnitOfWork _unitOfWork;

        public MilestoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
