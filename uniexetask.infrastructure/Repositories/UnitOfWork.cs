using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;

namespace uniexetask.infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniExetaskContext _dbContext;
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IRoleRepository Roles { get; }
        public IFeatureRepository Features { get; }
        public IPermissionRepository Permissions { get; }

        public UnitOfWork(UniExetaskContext dbContext,
                            IUserRepository userRepository,
                            IRefreshTokenRepository refreshTokens,
                            IRoleRepository roles,
                            IFeatureRepository features,
                            IPermissionRepository permissions)
        {
            _dbContext = dbContext;
            Users = userRepository;
            RefreshTokens = refreshTokens;
            Roles = roles;
            Features = features;
            Permissions = permissions;
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
