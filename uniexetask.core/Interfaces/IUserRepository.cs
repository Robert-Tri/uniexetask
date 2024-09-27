using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.core.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> AuthenticateAsync(string email, string password);
    }
}
