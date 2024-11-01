using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ProjectScoreService : IProjectScoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProjectScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddProjecScore(ProjectScore projecScore)
        {
            await _unitOfWork.ProjectScores.InsertAsync(projecScore);
            var result = _unitOfWork.Save();
            if (result > 0)
                return true;
            return false;
        }
    }
}
