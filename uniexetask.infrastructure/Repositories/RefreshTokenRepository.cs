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
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(UniExetaskContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<RefreshToken>> GetRefreshTokensByUserId(int userId)
        {
            return await dbSet.Where(u => u.UserId == userId && u.Status == true).ToListAsync();
        }

        public async Task<RefreshToken?> CheckRefreshTokenAsync(string refreshToken)
        {
            return await dbSet.FirstOrDefaultAsync(u =>
                u.Token.Equals(refreshToken) && u.Expires > DateTime.Now && u.Status == true);
        }
    }
}
