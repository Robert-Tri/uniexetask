using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<List<RefreshToken>> GetRefreshTokensByUserId(int id);
        Task<RefreshToken?> CheckRefreshTokenAsync(string refreshToken);
        Task<IEnumerable<RefreshToken>> GetAllActiveRefreshTokens();
    }
}
