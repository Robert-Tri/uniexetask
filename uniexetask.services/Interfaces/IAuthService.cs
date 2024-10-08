using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> GetUserByRefreshToken(string? refreshToken);
        Task<User?> LoginAsync(string username, string password);
        System.Threading.Tasks.Task SaveRefreshToken(int id, string refreshToken);
        Task<User?> GetUserByEmailAsync(string email);
        System.Threading.Tasks.Task ClearRefreshToken(int userId);

    }
}
