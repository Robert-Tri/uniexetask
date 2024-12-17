using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ReqMemberService : IReqMemberService
    {
        public IUnitOfWork _unitOfWork;
        public ReqMemberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RegMemberForm>> GetAllReqMember()
        {
            var reqMemberList = await _unitOfWork.ReqMembers.GetAsync(
                filter: rm => rm.Status == true, 
                includeProperties: "Group,Group.GroupMembers,Group.Subject,Group.GroupMembers.Student.User"
            );
            return reqMemberList;
        }

        public async Task<IEnumerable<RegMemberForm>> GetAllReqMemberByCampus(List<int> groupIds)
        {
            var reqMemberList = await _unitOfWork.ReqMembers.GetAsync(
                filter: rm => rm.Status == true && groupIds.Contains(rm.GroupId),
                includeProperties: "Group,Group.GroupMembers,Group.Subject,Group.GroupMembers.Student.User"
            );
            return reqMemberList;
        }

        public async Task<bool> CreateReqMember(RegMemberForm reqMember)
        {
            if (reqMember != null)
            {
                await _unitOfWork.ReqMembers.InsertAsync(reqMember);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<RegMemberForm?> GetReqMemberById(int id)
        {
            if (id > 0)
            {
                var RegMember = await _unitOfWork.ReqMembers.GetByIDAsync(id);
                if (RegMember != null)
                {
                    return RegMember;
                }
            }
            return null;
        }

        public async Task<IEnumerable<RegMemberForm>?> GetReqMembersByGroupId(int groupId)
        {
            if (groupId > 0)
            {
                var reqMemberList = await _unitOfWork.ReqMembers.GetAsync(filter: rm => rm.GroupId == groupId && rm.Status == true,
                includeProperties: "Group,Group.GroupMembers,Group.Subject,Group.GroupMembers.Student.User");
                if (reqMemberList != null && reqMemberList.Any())
                {
                    return reqMemberList;
                }
            }
            return null;
        }



        public async Task<bool> UpdateReqMember(RegMemberForm ReqMembers)
        {
            if (ReqMembers != null)
            {
                var RegMember = await _unitOfWork.ReqMembers.GetByIDAsync(ReqMembers.RegMemberId);
                if (RegMember != null)
                {
                    RegMember.GroupId = ReqMembers.GroupId;
                    RegMember.Description = ReqMembers.Description;
                    RegMember.Status = ReqMembers.Status;


                    _unitOfWork.ReqMembers.Update(RegMember);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

    }
}
