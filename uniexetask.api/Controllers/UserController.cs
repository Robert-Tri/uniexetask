using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using OfficeOpenXml;
using System.Security.Claims;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;
using uniexetask.shared.Extensions;
using Microsoft.AspNetCore.SignalR;
using uniexetask.services.Hubs;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ICampusService _campusService;
        private readonly ISubjectService _subjectService;
        private readonly IStudentService _studentsService;
        private readonly IUserService _userService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly Cloudinary _cloudinary;
        private readonly IHubContext<UserHub> _hubContext;
        public UserController(IRoleService roleService, ISubjectService subjectService, ICampusService campusService, IUserService userService, IMentorService mentorService, IMapper mapper, IEmailService emailService, IStudentService studentsService, IHubContext<UserHub> hubContext)
        {
            _roleService = roleService;
            _subjectService = subjectService;
            _campusService = campusService;
            _studentsService = studentsService;
            _mentorService = mentorService;
            _userService = userService;
            _emailService = emailService;
            _mapper = mapper;
            _hubContext = hubContext;

            var cloudinaryAccount = new Account(
               "dan0stbfi",  
               "687237422157452",
               "XQbEo1IhkXxbC24rHvpdNJ5BHNw"   
           );
            _cloudinary = new Cloudinary(cloudinaryAccount);

        }

        [HttpGet("student/search-studentcode")]
        public async Task<IActionResult> SearchStudentByStudentCode([FromQuery] string query)
        {
            ApiResponse<IEnumerable<SearchStudentCodeResponse>> response = new ApiResponse<IEnumerable<SearchStudentCodeResponse>>();
            try
            {
                
                List<SearchStudentCodeResponse> studentList = new List<SearchStudentCodeResponse>();
                if (string.IsNullOrWhiteSpace(query))
                {
                    throw new Exception("Query parameter is required.");
                }
            
                var students = await _userService.SearchStudentByStudentCodeAsync(query);

                if (students.Count() == 0)
                {
                    throw new Exception("No matching student codes found.");
                }

                foreach (var student in students)
                {
    #pragma warning disable CS8601 // Possible null reference assignment.
                    studentList.Add(new SearchStudentCodeResponse 
                    { 
                        FullName = student.FullName,
                        Email = student.Email,
                        StudentCode = student.Students.FirstOrDefault()?.StudentCode,
                        Avatar = student.Avatar,
                    });
    #pragma warning restore CS8601 // Possible null reference assignment.
                }
                response.Data = studentList;

                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = e.Message;
                return Ok(response);
            }

        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        //[Authorize(Policy = "CanViewUser")]
        [Authorize(Policy = "CanViewUser")]
        [HttpGet]
        public async Task<IActionResult> GetUserList()
        {
            var usersList = await _userService.GetAllUsers();
            if (usersList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<User>> response = new ApiResponse<IEnumerable<User>>();
            response.Data = usersList;
            return Ok(response);
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            ApiResponse<User> response = new ApiResponse<User>();

            var user = await _userService.GetUserById(userId);
            try
            {
                if (user != null)
                {
                    response.Data = user;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("User can not find.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }

        [Authorize]
        [HttpGet("ProfileUser")]
        public async Task<IActionResult> ProfileUser()
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId)) 
            {
                var users = await _userService.GetUserByIdWithCampusAndRoleAndStudents(userId);

                if (users != null)
                {
                    return Ok(users); 
                }
                else
                {
                    return BadRequest("User not found"); 
                }
            }
            else
            {
                return BadRequest("Invalid userId"); 
            }
        }

        [HttpGet("view-profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            ApiResponse<User> response = new ApiResponse<User>();
            List<ChatMessageResponse> list = new List<ChatMessageResponse>();
            try
            {
                var user = await _userService.GetUserByIdWithCampusAndRoleAndStudents(userId);

                if (user != null)
                {
                    response.Data =user;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("student not found");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return Ok(response);
            }
        }

        [Authorize(Policy = "CanImportUser")]
        [HttpPost]
        [Route("upload-excel")]
        public async Task<IActionResult> CreateUser(IFormFile excelFile)
        {
            ApiResponse<IEnumerable<string>> response = new ApiResponse<IEnumerable<string>>();
            try
            {
                var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
                {
                    throw new Exception("Invalid User Id.");
                }
                if (excelFile == null || excelFile.Length == 0)
                {
                    throw new Exception("File is not selected or is empty.");
                }
                var errors = await _userService.ImportStudentFromExcel(userId, excelFile);

                if (errors != null)
                {
                    response.Data = errors;
                }
                return Ok(response);
            }
            catch (Exception e)
            {
                response.Success = false;
                response.ErrorMessage = e.Message;
                return BadRequest(response);
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

        private static Dictionary<string, string> VerificationCodes = new Dictionary<string, string>();

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                string randomCode = GenerateRandomCode(5);

                VerificationCodes[email] = randomCode;

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
                if (VerificationCodes.ContainsKey(request.Email))
                {
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



        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [Authorize(Policy = "CanCreateUser")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel user)
        {
            var isUserExisted = await _userService.CheckDuplicateUser(user.Email, user.Phone);
            if (isUserExisted)
                return Conflict("Email or phone has already been registered!");
            var obj = _mapper.Map<User>(user);
            string password = PasswordHasher.GenerateRandomPassword(10);
            obj.Password = PasswordHasher.HashPassword(password);
            obj.Avatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg";
            var isUserCreated = await _userService.CreateUser(obj);

            if (isUserCreated != null)
            {
                var userEmail = $@"
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
    <h2>Dear {user.FullName},</h2>
    <p>We are sending you the following login account information:</p>
    <ul>
        <li><strong>Account:</strong> {user.Email}</li>
        <li><strong>Password: </strong><em>{password}</em>.</li>
    </ul>
    <p>We recommend that you change your password after logging in for the first time.</p>
    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>
    <p>This is an automated email, please do not reply to this email. If you need assistance, please contact us at unitask68@gmail.com.</p>
</body>
</html>
";
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _emailService.SendEmailAsync(user.Email, "Account UniEXETask", userEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                return Ok(isUserCreated);
            }
            else
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Update the product
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [Authorize(Policy = "CanEditUser")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserUpdateModel user)
        {
            var isExistedUser = await _userService.CheckDuplicateUserForUpdate(user.UserId, user.Email, user.Phone);
            if (isExistedUser)
                return Conflict("Email or phone has already been registered!");
            ApiResponse<bool> respone = new ApiResponse<bool>();
            var isUserUpdated = await _userService.UpdateUser(_mapper.Map<User>(user));
            if (isUserUpdated)
            {
                respone.Data = isUserUpdated;
                return Ok(respone);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel userProfile)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.FullName = userProfile.FullName;
            user.Phone = userProfile.Phone;
            var isUserUpdated = await _userService.UpdateUser(user);
            ApiResponse<object> response = new ApiResponse<object> { Data = new
            {
                FullName = user.FullName,
                Phone = user.Phone
            }
            };
            if (isUserUpdated)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest("Failed to update password.");
            }
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel passwordModel)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.Password != null && !PasswordHasher.VerifyPassword(passwordModel.oldPassword, user.Password))
            {
                return BadRequest(new ApiResponse<object> { Success = false, ErrorMessage = "Old password is incorrect." });
            }

            if (!IsPasswordValid(passwordModel.newPassword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    ErrorMessage = "Password must be at least 8 characters long, and include at least one uppercase letter, one lowercase letter, and one number."
                });
            }

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



        [Authorize]
        [HttpPut("UploadProfileAvatar")]
        public async Task<IActionResult> UploadProfileAvatar(IFormFile file)
        {
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new ApiResponse<bool> { Success = false, ErrorMessage = "Unauthorized access." });
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<bool> { Success = false, ErrorMessage = "User not found." });
            }

            if (file != null && file.Length > 0)
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return BadRequest(new ApiResponse<bool> { Success = false, ErrorMessage = uploadResult.Error.Message });
                }

                string oldAvatar = user.Avatar;
                user.Avatar = uploadResult.SecureUri.ToString();

                if (!string.IsNullOrEmpty(oldAvatar) && !oldAvatar.Contains("https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg"))
                {
                    string publicId = GetPublicIdFromUrl(oldAvatar);
                    var deletionParams = new DeletionParams(publicId);
                    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                    if (deletionResult.Error != null)
                    {
                        return BadRequest(new ApiResponse<bool> { Success = false, ErrorMessage = deletionResult.Error.Message });
                    }
                }
            }

            var isUserUpdated = await _userService.UpdateUser(user);
            if (!isUserUpdated)
            {
                return BadRequest(new ApiResponse<bool> { Success = false, ErrorMessage = "Failed to update user information." });
            }

            ApiResponse<string> response = new ApiResponse<string> { Data = user.Avatar };
            return Ok(response);
        }

        private string GetPublicIdFromUrl(string cloudinaryUrl)
        {
            var uri = new Uri(cloudinaryUrl);
            var parts = uri.Segments;
            var publicIdWithExtension = parts[parts.Length - 1].Split('.')[0];
            return publicIdWithExtension;
        }




        /// <summary>
        /// Delete product by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Policy = "CanDeleteUser")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteProduct(int userId)
        {
            ApiResponse<bool> respone = new ApiResponse<bool>();
            var isUserDeleted = await _userService.DeleteUser(userId);

            if (isUserDeleted)
            {
                respone.Data = true;
                return Ok(respone);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
