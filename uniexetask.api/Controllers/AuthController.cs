using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using Google.Apis.Auth;
using uniexetask.shared.Extensions;

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
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration,
            IAuthService authService,
            IRoleService roleService,
            IRolePermissionService rolePermissionService,
            IEmailService emailService,
            IUserService userService)
        {
            _configuration = configuration;
            _authService = authService;
            _roleService = roleService;
            _rolePermissionService = rolePermissionService;
            _emailService = emailService;
            _userService = userService;
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

                var user = await _authService.GetUserByEmailAsync(model.Email);
                if (user == null) throw new Exception("Email is incorrect or not registered.");
                if (user.Password != null && !PasswordHasher.VerifyPassword(model.Password, user.Password)) throw new Exception("Email or password is not correct.");
                TokenModel token = new TokenModel
                {
                    AccessToken = await GenerateAccessToken(user),
                    RefreshToken = await GenerateRefreshToken()
                };
                await _authService.SaveRefreshToken(user.UserId, token.RefreshToken);
                response.Data = token.AccessToken;

                Response.Cookies.Append("AccessToken", token.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(7)
                });

                Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(7)
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


        [HttpPost("hashpassword")]
        public IActionResult HashPassword([FromBody] string password)
        {
            return Ok(PasswordHasher.HashPassword(password));
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
                Response.Cookies.Append("AccessToken", "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(-1)
                });

                Response.Cookies.Append("RefreshToken", "", new CookieOptions
                {
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    HttpOnly = true,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(-1)
                });

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
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Uri, user.Avatar != null? user.Avatar : "")

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
                response.Data = token.AccessToken;

                // Set cookies
                Response.Cookies.Append("AccessToken", token.AccessToken, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(7)
                });

                Response.Cookies.Append("RefreshToken", token.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.Now.AddDays(7)
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


        private static Dictionary<string, string> VerificationCodes = new Dictionary<string, string>();

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                // Tạo mã ngẫu nhiên 5 chữ số
                string randomCode = GenerateRandomCode(5);

                // Lưu mã vào bộ nhớ (có thể lưu theo email hoặc một số nhận dạng khác)
                VerificationCodes[email] = randomCode;

                // Nội dung email bao gồm mã ngẫu nhiên
                var contentEmail = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            padding: 20px;
            background-color: #ffffff;
        }}
        h2 {{
            color: #333;
        }}
        p {{
            margin: 0 0 10px;
        }}
    </style>
</head>
<body>
    <p>We have received your request to reset your password. Please use the following code to proceed:</p>
    <p><strong>{randomCode}</strong></p>
    <p>You should not share this code with anyone.</p>
    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>
    <p>This is an automated email, please do not reply to this email. If you need assistance, please contact us at unitask68@gmail.com.</p>
</body>
</html>
";

                var emailTask = _emailService.SendEmailAsync(email, "Password Reset Request", contentEmail);

                if (emailTask != null)
                {
                    return Ok(new { message = "Password reset email sent successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to send email." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }

        [HttpPost("verifyCode")]
        public IActionResult VerifyCode([FromBody] VerifyCodeModel request)
        {
            try
            {
                // Kiểm tra mã xác minh có tồn tại trong bộ nhớ không
                if (VerificationCodes.ContainsKey(request.Email))
                {
                    // Kiểm tra mã xác minh có khớp không
                    if (VerificationCodes[request.Email] == request.Code)
                    {
                        return Ok(new { message = "Code verified successfully." });
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid code." });
                    }
                }
                else
                {
                    return BadRequest(new { message = "No code found for this email." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", details = ex.Message });
            }
        }


        private string GenerateRandomCode(int length)
        {
            const string digits = "0123456789";

            var random = new Random();

            var code = new List<char>();

            for (int i = 0; i < length; i++)
            {
                code.Add(digits[random.Next(digits.Length)]);
            }

            return new string(code.ToArray());
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel passwordModel, string email)
        {
            var user = await _userService.GetUserByEmail(email);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (passwordModel.newPassword != null)
            {
                user.Password = PasswordHasher.HashPassword(passwordModel.newPassword);
                var isUserUpdated = await _userService.UpdateUser(user);

                ApiResponse<object> response = new ApiResponse<object>
                {
                    Data = new
                    {
                        Password = user.Password
                    }
                };
                if (isUserUpdated)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest("Không thể cập nhật mật khẩu.");
                }
            }
            return BadRequest("New Password is null.");

        }
    }
}
