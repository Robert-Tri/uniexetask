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
        Task<User> CreateUser(User user);
        Task<User> CreateUserExcel(User user);
        Task<bool> CheckDuplicateUser(string email, string phone);

        Task<IEnumerable<User>> GetAllUsers();

        Task<User?> GetUserById(int userId);
        Task<User?> GetUserByIdWithCampusAndRoleAndStudents(int userId);

        Task<bool> UpdateUser(User userDetails);

        Task<bool> DeleteUser(int userId);
        Task<IEnumerable<User>> SearchUsersByEmailAsync(string query);
        Task<IEnumerable<User>> SearchStudentByStudentCodeAsync(string query);
    }
}
