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
    class StudentRepsitory : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepsitory(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Student>> GetEligibleStudentsWithUser()
        {
            return await dbSet.Where(s => s.IsCurrentPeriod == true).Include(s => s.User).ToListAsync();
        }

        public async Task<Student?> GetStudentByUserId(int inviteeId)
        {
            return await dbSet
                .Where(s => s.UserId == inviteeId)
                .FirstOrDefaultAsync();
        }
    }
}
