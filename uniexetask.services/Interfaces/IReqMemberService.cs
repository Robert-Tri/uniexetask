using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IReqMemberService
    {
        Task<IEnumerable<RegMemberForm>> GetAllReqMember();
        Task<bool> CreateReqMember(RegMemberForm reqMember);
        Task<RegMemberForm?> GetReqMemberById(int id);
        Task<bool> UpdateReqMember(RegMemberForm ReqMembers);
    }
}
