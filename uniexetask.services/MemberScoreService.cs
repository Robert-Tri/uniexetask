using uniexetask.core.Interfaces;
using uniexetask.core.Models;
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

        public async Task<bool> AddMemberScore(List<MemberScore> memberScores)
        {
            foreach (var memberScore in memberScores) 
            {
                await _unitOfWork.MemberScores.InsertAsync(memberScore);
            }
            var result = _unitOfWork.Save();
            if(result > 0)
                return true;
            return false;
        }
    }
}
