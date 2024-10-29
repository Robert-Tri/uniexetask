using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class MemberScoreService : IMemberScoreService
    {
        public IUnitOfWork _unitOfWork;

        public MemberScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    }
}
