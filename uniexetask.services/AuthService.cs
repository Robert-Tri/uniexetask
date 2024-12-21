using FirebaseAdmin.Auth.Hash;
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
        public IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<User?> GetUserByRefreshToken(string refreshToken = "")
        {
            var rt = await _unitOfWork.RefreshTokens.CheckRefreshTokenAsync(refreshToken);
            if (rt == null)
            {
                return null;
            }
            var user = await _unitOfWork.Users.GetByIDAsync(rt.UserId);
            if (user == null) 
            {
                return null;
            }
            return user;
        }

        public async System.Threading.Tasks.Task SaveRefreshToken(int id, string refreshToken)
        {
            await RevokeRefreshToken(id);
            var newRefreshToken = new RefreshToken
            {
                UserId = id,
                Expires = DateTime.Now.AddDays(7),
                Revoked = DateTime.Now.AddDays(7),
                Token = refreshToken,
                Status = true
            };
            await _unitOfWork.RefreshTokens.InsertAsync(newRefreshToken);

            await _unitOfWork.SaveAsync();

        }

        public async Task<bool>  RevokeRefreshToken(int userId)
        {
            var rts = await _unitOfWork.RefreshTokens.GetRefreshTokensByUserId(userId);
            if (rts != null)
            {
                foreach (var token in rts)
                {
                    token.Status = false;
                    token.Revoked = DateTime.Now;
                    _unitOfWork.RefreshTokens.Update(token);
                }
                await _unitOfWork.SaveAsync();
                return true;
            }
            return false;

        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {

            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Email is not exists or not correct.");
            }

            if (user.IsDeleted) throw new Exception("This account has be deleted.");

            return user;
        }
    }
}
