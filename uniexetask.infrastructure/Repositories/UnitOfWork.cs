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

        public UnitOfWork(UniExetaskContext dbContext,
                            IUserRepository userRepository)
        {
            _dbContext = dbContext;
            Users = userRepository;
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

    }
}
