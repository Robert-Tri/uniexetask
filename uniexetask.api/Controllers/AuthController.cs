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

namespace uniexetask.api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public readonly IAuthService _authService;
        public readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IAuthService authService, IUserService userService)
        {
            _configuration = configuration;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            ApiResponse<TokenModel> response = new ApiResponse<TokenModel>();
            var user = await _authService.LoginAsync(model.Email, model.Password);
            if (user != null)
            {
                TokenModel token = new TokenModel
                {
                    AccessToken = GenerateAccessToken(user),
                    RefreshToken = GenerateRefreshToken()
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
            response.Success = false;
            response.ErrorMessage = "Authentication failed!";
            return Unauthorized(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel model)
        {
            ApiResponse<TokenModel> response = new ApiResponse<TokenModel>();
            var user = await _authService.GetUserByRefreshToken(model.RefreshToken);

            if (user == null)
                return Unauthorized("Your session has expired. Please log in again.");
            var newAccessToken = GenerateAccessToken(user);
            Response.Cookies.Append("AccessToken", newAccessToken ?? "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
            return Ok(newAccessToken);
        }

        private string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key must be configured")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var token = new JwtSecurityToken(
                        issuer: _configuration["Jwt:Issuer"],
                        audience: _configuration["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpirationMinutes"])),
                        signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
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
                    AccessToken = GenerateAccessToken(user),
                    RefreshToken = GenerateRefreshToken()
                };

                await _authService.SaveRefreshToken(user.UserId, token.RefreshToken);
                response.Data = token;

                // Set cookies
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
            catch (InvalidJwtException)
            {
                response.Success = false;
                response.ErrorMessage = "Invalid Google token.";
                return Unauthorized(response);
            }
        }

        [Authorize]
        [HttpGet("userinfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                var userId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var userRole = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var user = await _userService.GetUserById(Convert.ToInt32(userId));
                return Ok(new
                {
                    userId,
                    email,
                    role = userRole,
                    fullname = user.FullName,
                    rolename = user.Role.Name
                });
            }
            return Unauthorized();
        }


    }
}
