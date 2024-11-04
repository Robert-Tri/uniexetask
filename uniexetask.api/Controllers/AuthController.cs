using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;
using uniexetask.core.Models;
using Azure;
using Google.Apis.Auth;

using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Primitives;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;

namespace uniexetask.api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IRoleService _roleService;
        private readonly IRolePermissionService _rolePermissionService;

        public AuthController(IConfiguration configuration, IAuthService authService, IRoleService roleService, IRolePermissionService rolePermissionService)
        {
            _configuration = configuration;
            _authService = authService;
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            ApiResponse<TokenModel> response = new ApiResponse<TokenModel>();
            try
            {
                var user = await _authService.LoginAsync(model.Email, model.Password);
                if (user == null) throw new Exception("Email or password is not correct.");
                TokenModel token = new TokenModel
                {
                    AccessToken = await GenerateAccessToken(user),
                    RefreshToken = await GenerateRefreshToken()
                };
                await _authService.SaveRefreshToken(user.UserId, token.RefreshToken);
                response.Data = token;

                Response.Cookies.Append("AccessToken", token.AccessToken ?? "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });
                Response.Cookies.Append("RefreshToken", token.RefreshToken ?? "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30)
                });

                    return Ok(response);
            }
            catch (Exception ex) 
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Unauthorized(response);
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    throw new Exception("User not authenticated.");
                }

                var userId = int.Parse(userIdClaim);
                await _authService.RevokeRefreshToken(userId);
                Response.Cookies.Delete("AccessToken");
                Response.Cookies.Delete("RefreshToken");

                response.Data = "Logout successful.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel model)
        {
            ApiResponse<TokenModel> response = new ApiResponse<TokenModel>();
            var user = await _authService.GetUserByRefreshToken(model.RefreshToken);

            if (user == null)
                return Unauthorized("Your session has expired. Please log in again.");
            var newAccessToken = await GenerateAccessToken(user);
            Response.Cookies.Append("AccessToken", newAccessToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(1)
            });
            return Ok(newAccessToken);
        }

        private async Task<string> GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key must be configured")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var role = await _roleService.GetRoleById(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.Name),
                new Claim(ClaimTypes.Name, user.FullName)
            };
            var permissions = await _rolePermissionService.GetRolePermissionsByRole(role.Name);
            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim("permissions", permission.Name));
                }
            }

            var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                        signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static async Task<string> GenerateRefreshToken()
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                var randomNumber = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    return Convert.ToBase64String(randomNumber);
                }
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            ApiResponse<TokenModel> response = new ApiResponse<TokenModel>();

            try
            {
                // Validate Google Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token);

                // Check if the email exists in the database
                var user = await _authService.GetUserByEmailAsync(payload.Email);

                if (user == null)
                {
                    response.Success = false;
                    response.ErrorMessage = "Email not registered.";
                    return Unauthorized(response);
                }

                // Generate tokens
                TokenModel token = new TokenModel
                {
                    AccessToken = await GenerateAccessToken(user),
                    RefreshToken = await GenerateRefreshToken()
                };

                await _authService.SaveRefreshToken(user.UserId, token.RefreshToken);
                response.Data = token;

                // Set cookies
                Response.Cookies.Append("AccessToken", token.AccessToken ?? "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                });
                Response.Cookies.Append("RefreshToken", token.RefreshToken ?? "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(30)
                });

                return Ok(response);
            }
            catch (InvalidJwtException)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid Google token.";
                return Unauthorized(response);
            }
        }
    }
}
