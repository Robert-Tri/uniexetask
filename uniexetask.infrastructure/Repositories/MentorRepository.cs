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
    public class MentorRepository : GenericRepository<Mentor>, IMentorRepository
    {
        public MentorRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<Mentor?> GetMentorByUserId(int userId)
        {
            return await dbSet.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Mentor?> GetMentorWithGroupAsync(int mentorId)
        {
            return await dbSet
                .Include(r => r.Groups)
                .FirstOrDefaultAsync(r => r.MentorId == mentorId);
        }
        public async Task<IEnumerable<Mentor>> GetMentorsWithCampus()
        {
            return await dbSet.Include(m => m.User).AsNoTracking().ToListAsync();
        }
    }
}
