using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class UserService : IUserService
    {
        public IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CheckDuplicateUser(string email, string phone)
        {
            var users = (await _unitOfWork.Users.GetAsync(filter: u => (u.Email == email || u.Phone == phone) && u.IsDeleted == false));
            if (users.Any())
            {
                return true;
            }
            return false;
        } 
        
        public async Task<bool> CheckDuplicateUserForUpdate(int userId, string email, string phone)
        {
            var users = await _unitOfWork.Users.GetAsync(filter: u => u.UserId != userId);
            var existedUser = users.Where(u => (u.Email == email || u.Phone == phone) && u.IsDeleted == false);
            if (existedUser.Any())
                return true;
            return false;
        }

        public async Task<User> CreateUser(User user)
        {
            var existedUser = await CheckDuplicateUser(user.Email, user.Phone);
            if (user != null)
            {
                await _unitOfWork.Users.InsertAsync(user);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return user;
            }
            throw new Exception("Failed to create user.");
        }

        public async Task<User> CreateUserExcel(User user)
        {
            var existedUser = await CheckDuplicateUser(user.Email, user.Phone);
            if (existedUser)
            {
                throw new Exception("Email or phone number already exists.");
            }

            if (user != null)
            {
                await _unitOfWork.Users.InsertAsync(user);

                var result = _unitOfWork.Save();

                if (result > 0)
                {
                    return user;
                }
            }

            throw new Exception("Failed to create user.");
        }


        public async Task<bool> DeleteUser(int userId)
        {

            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDAsync(userId);
                if (users != null)
                {
                    users.IsDeleted = true; 
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            //var usersList = await _unitOfWork.Users.GetAsync(includeProperties: "Campus,Role");
            var usersList = await _unitOfWork.Users.GetUsersWithCampusAndRole();
            return usersList;
        }

        public async Task<User?> GetUserById(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDAsync(userId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<IEnumerable<User>> SearchUsersByEmailAsync(string query)
        {
            return await _unitOfWork.Users.SearchUsersByEmailAsync(query);
        }
        
        public async Task<User?> GetUserByIdWithCampusAndRoleAndStudents(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDWithCampusAndRoleAndStudents(userId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<bool> UpdateUser(User user)
        {

            if (user != null)
            {
                var updatedUser = await _unitOfWork.Users.GetByIDAsync(user.UserId);
                if (updatedUser != null)
                {
                    updatedUser.FullName = user.FullName;
                    updatedUser.Email = user.Email;
                    updatedUser.Password = user.Password;
                    updatedUser.Phone = user.Phone;
                    updatedUser.CampusId = user.CampusId;

                    _unitOfWork.Users.Update(updatedUser);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<User>> SearchStudentByStudentCodeAsync(string query)
        {
            return await _unitOfWork.Users.SearchStudentsByStudentCodeAsync(query);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            if (email != null)
            {
                var users = await _unitOfWork.Users.GetUserByEmailAsync(email);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }
    }
}
