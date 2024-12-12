using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;

namespace uniexetask.infrastructure.Repositories
{
    class ReqMemberRepsitory : GenericRepository<RegMemberForm>, IReqMemberRepository
    {
        public ReqMemberRepsitory(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async System.Threading.Tasks.Task DeleteReqMemberForm(int groupId)
        {
            var reqMemberForms = await dbSet.Where(r => r.GroupId == groupId).ToListAsync();
            if (reqMemberForms.Any()) 
            {
                foreach (var reqMemberForm in reqMemberForms)
                {
                    dbSet.Remove(reqMemberForm);
                }
            }
        }

        public async System.Threading.Tasks.Task DeleteRegMemberForm(int groupId)
        {
            var reqMemberForms = await dbSet.Where(r => r.GroupId == groupId).ToListAsync();
            if (reqMemberForms.Any())
            {
                foreach(var reqMemberForm in reqMemberForms)
                {
                    reqMemberForm.Status = false;
                    dbSet.Update(reqMemberForm);
                }
            }
        }
    }
}
