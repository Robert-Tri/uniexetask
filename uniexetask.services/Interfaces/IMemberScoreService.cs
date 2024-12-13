using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IMemberScoreService
    {
        Task<bool> AddMemberScore(List<MemberScore> memberScores);
        Task<MemberScoreResult> GetMemberScores(int projectId, int milestoneId);
        Task<TotalMemberScoreResult> GetTotalMemberScore(int projectId);
        Task<TotalMemberScoreResult> GetTotalMemberScoreV2(int projectId);
    }
}
