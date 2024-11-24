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
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                if (!IsPasswordValid(model.Password))
                {
                    throw new Exception("Password must be at least 8 characters long, and include at least one uppercase letter, one lowercase letter, and one number.");
                }
                var user = await _authService.LoginAsync(model.Email, model.Password);
                if (user == null) throw new Exception("Email or password is not correct.");
                TokenModel token = new TokenModel
                {
                    AccessToken = await GenerateAccessToken(user),
                    RefreshToken = await GenerateRefreshToken()
                };
                await _authService.SaveRefreshToken(user.UserId, token.RefreshToken);
                response.Data = "Login successfully!";

                Response.Cookies.Append("AccessToken", token.AccessToken, new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
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

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            if (password.Length < 8) return false;

            if (!password.Any(char.IsUpper)) return false;

            if (!password.Any(char.IsLower)) return false;

            if (!password.Any(char.IsDigit)) return false;

            return true;
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
                var result = await _authService.RevokeRefreshToken(userId);
                if (!result) throw new Exception("Logout failed");
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
/*            Response.Cookies.Append("AccessToken", newAccessToken ?? "", new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(1)
            });*/
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
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                // Validate Google Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(model.Token);

                if (payload == null) throw new Exception("Invalid Google Token");

                // Check if the email exists in the database
                var user = await _authService.GetUserByEmailAsync(payload.Email);

                if (user == null)
                {
                    throw new Exception("Email not registered.");
                }

                // Generate tokens
                TokenModel token = new TokenModel
                {
                    AccessToken = await GenerateAccessToken(user),
                    RefreshToken = await GenerateRefreshToken()
                };

                await _authService.SaveRefreshToken(user.UserId, token.RefreshToken);
                response.Data = "Login successfully!";

                // Set cookies
                Response.Cookies.Append("AccessToken", token.AccessToken, new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(7)
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
