using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IMilestoneService
    {
        Task<IEnumerable<Milestone>> GetMileStones();
        Task<IEnumerable<Milestone>> GetMileStonesBySubjectId(int subjectId);
        Task<Milestone?> GetMilestoneWithCriteria(int id);
        Task<IEnumerable<Milestone>> GetUndeleteMileStonesBySubjectId(int subjectId);
        Task<Milestone?> GetUndeleteMilestoneWithCriteria(int id);
        Task<bool> ImportMilestoneWithCriteriaFromExcel(IFormFile excelFile);
    }
}
