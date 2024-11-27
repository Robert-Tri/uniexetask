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
            return await dbSet
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByIDWithCampusAndRoleAndStudents(int id)
        {
            return await dbSet
       .Include(u => u.Role)                 // Bao gồm bảng Role
       .Include(u => u.Campus)               // Bao gồm bảng Campus
       .Include(u => u.Students)             // Bao gồm bảng Student
       .FirstOrDefaultAsync(u => u.UserId == id); // Điều kiện truy vấn dựa trên UserId
        }


        public async Task<IEnumerable<User>> GetUsersWithCampusAndRole()
        {
            return await dbSet.Include(u => u.Campus).Include(u => u.Role).AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetUserWithChatGroupByUserIdAsyn(int userId)
        {
            return await dbSet.Include(i => i.ChatGroups)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> SearchUsersByEmailAsync(string query)
        {
            return await dbSet
                        .Where(u => EF.Functions.Like(u.Email, $"%{query}%"))
                        .Take(5)
                        .ToListAsync();
        }

        public async Task<IEnumerable<User>> SearchStudentsByStudentCodeAsync(string query)
        {
            return await dbSet
                    .Include(u => u.Students) // Bao gồm thông tin từ bảng Student
                    .Where(u => u.Students.Any(s => EF.Functions.Like(s.StudentCode, $"%{query}%"))) // Lọc theo StudentCode
                    .Take(5)
                    .ToListAsync();
        }
    }
}
