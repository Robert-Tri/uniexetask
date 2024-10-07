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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(UniExetaskContext dbContext) : base(dbContext)
        {

        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await dbSet.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == email.ToLower() &&
                u.Password == password);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await dbSet.FirstOrDefaultAsync(u =>
                u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIDAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }
        public async Task<IEnumerable<User>> GetUsersWithCampusAndRole()
        {
            return await dbSet.Include(u => u.Campus).Include(u => u.Role).AsNoTracking().ToListAsync();
        }
    }
}
