using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IUserService
    {
        Task<bool> CreateUser(User userDetails);

        Task<IEnumerable<User>> GetAllUsers();

        Task<User?> GetUserById(int userId);
        Task<User?> GetUserByIdWithCampusAndRole(int userId);

        Task<bool> UpdateUser(User userDetails);

        Task<bool> DeleteUser(int userId);
        Task<IEnumerable<User>> SearchUsersByEmailAsync(string query);
    }
}
