using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IProjectScoreService
    {
        Task<bool> AddProjecScore(List<ProjectScore> projectScores);
        Task<double> GetMileStoneScore(int projectId, int mileStoneId);
    }
}
