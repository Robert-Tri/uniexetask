using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IReqMemberRepository : IGenericRepository<RegMemberForm>
    {
        public System.Threading.Tasks.Task DeleteReqMemberForm(int groupId);
    }
}
