using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uniexetask.core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICampusRepository Campus { get; }
        IRoleRepository Role { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        int Save();

        Task<int> SaveAsync();
    }
}
