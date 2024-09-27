using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _userRepository.AuthenticateAsync(email, password);
            if (user == null)
            {
                return null;
            }
            return user;
        }
    }
}
