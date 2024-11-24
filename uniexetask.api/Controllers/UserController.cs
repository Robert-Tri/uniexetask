using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using OfficeOpenXml;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;
using uniexetask.api.Extensions;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly Cloudinary _cloudinary;
        public UserController(IUserService userService, IMapper mapper, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
            _mapper = mapper;

            var cloudinaryAccount = new Account(
               "dan0stbfi",   // Cloud Name
               "687237422157452",      // API Key
               "XQbEo1IhkXxbC24rHvpdNJ5BHNw"    // API Secret
           );
            _cloudinary = new Cloudinary(cloudinaryAccount);

        }

        [HttpGet("search-email")]
        public async Task<IActionResult> SearchUserByEmail([FromQuery] string query)
        {
            ApiResponse<IEnumerable<UserModel>> response = new ApiResponse<IEnumerable<UserModel>>();
            List<UserModel> userList = new List<UserModel>();
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }
            
            var users = await _userService.SearchUsersByEmailAsync(query);

            if (users.Count() == 0)
            {
                return NotFound("No matching emails found.");
            }

            foreach (var use in users)
            {
                var userEntity = _mapper.Map<UserModel>(use);
                userList.Add(userEntity);
            }
            response.Data = userList;

            return Ok(response);
        }

        /// <summary>
        /// Get the list of product
        /// </summary>
        /// <returns></returns>
        //[Authorize(Policy = "CanViewUser")]
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


        [HttpPost]
        [Route("upload-excel")]
        public async Task<IActionResult> CreateUser(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                var response = new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = "File is not selected or is empty."
                };
                return BadRequest(response);
            }

            var usersList = new List<UserModel>();
            var emailList = new HashSet<string>();
            var phoneList = new HashSet<string>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var campusName = worksheet.Cells[row, 6].Text;
                        var roleName = worksheet.Cells[row, 8].Text;
                        var khoiText = worksheet.Cells[row, 9].Text;

                        int campusId = campusName switch
                        {
                            "FPT-HN" => 1,
                            "FPT-HCM" => 2,
                            "FPT-DN" => 3,
                            _ => 0
                        };

                        int roleId = roleName.ToLower() switch
                        {
                            "admin" => 1,
                            "manager" => 2,
                            "student" => 3,
                            "mentor" => 4,
                            "sponsor" => 5,
                            _ => 0
                        };

                        string password = worksheet.Cells[row, 3].Text;
                        string fullname = worksheet.Cells[row, 2].Text;
                        string emailUser = worksheet.Cells[row, 4].Text;
                        string phoneUser = worksheet.Cells[row, 5].Text;
                        int khoiNumber = int.Parse(khoiText.Substring(1));

                        if (emailList.Contains(emailUser))
                        {
                            var response = new ApiResponse<bool>
                            {
                                Success = false,
                                ErrorMessage = $"Duplicate email found: {emailUser}"
                            };
                            return BadRequest(response);
                        }
                        emailList.Add(emailUser);

                        if (!emailUser.Contains("@"))
                        {
                            var response = new ApiResponse<bool>
                            {
                                Success = false,
                                ErrorMessage = $"Invalid email format: {emailUser}. Email must contain '@'."
                            };
                            return BadRequest(response);
                        }

                        if (phoneList.Contains(phoneUser))
                        {
                            var response = new ApiResponse<bool>
                            {
                                Success = false,
                                ErrorMessage = $"Duplicate phone number found: {phoneUser}"
                            };
                            return BadRequest(response);
                        }
                        phoneList.Add(phoneUser);

                        if (!long.TryParse(phoneUser, out _))
                        {
                            var response = new ApiResponse<bool>
                            {
                                Success = false,
                                ErrorMessage = $"Invalid phone number (should be numeric): {phoneUser}"
                            };
                            return BadRequest(response);
                        }

                        if (khoiNumber >= 19)
                        {
                            password = GenerateRandomPassword(8);
                        }

                        var user = new UserModel
                        {
                            FullName = fullname,
                            Password = password, // Lưu mật khẩu gốc để gửi email
                            Email = emailUser,
                            Phone = phoneUser,
                            CampusId = campusId,
                            Status = bool.Parse(worksheet.Cells[row, 7].Text),
                            RoleId = roleId
                        };

                        usersList.Add(user);
                    }
                }
            }

            foreach (var userModel in usersList)
            {
                // Lưu mật khẩu gốc để gửi qua email
                string rawPassword = userModel.Password;

                // Hash mật khẩu trước khi lưu vào cơ sở dữ liệu
                userModel.Password = PasswordHasher.HashPassword(userModel.Password);

                var userEntity = _mapper.Map<User>(userModel);

                var isUserCreated = await _userService.CreateUser(userEntity);

                if (!isUserCreated)
                {
                    var response = new ApiResponse<bool>
                    {
                        Success = false,
                        ErrorMessage = $"Failed to create user: {userModel.Email}"
                    };
                    return BadRequest(response);
                }

                // Gửi mật khẩu gốc qua email
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
    <h2>Dear {userModel.FullName},</h2>
    <p>We sent you the login password: <strong>{rawPassword}</strong>. We recommend that you change your password after logging in for the first time.</p>
    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>
    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";

                await _emailService.SendEmailAsync(userModel.Email, "Users Invitation", userEmail);
            }

            var successResponse = new ApiResponse<string>
            {
                Success = true,
                Data = "All users were successfully created."
            };

            return Ok(successResponse);
        }



        private string GenerateRandomPassword(int length)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            var random = new Random();

            var password = new List<char>
            {
                upperChars[random.Next(upperChars.Length)], 
                lowerChars[random.Next(lowerChars.Length)], 
                digits[random.Next(digits.Length)],
            };

            const string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = password.Count; i < length; i++)
            {
                password.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(password.OrderBy(c => random.Next()).ToArray());
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



        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel users)
        {
            var obj = _mapper.Map<User>(users);
            var isUserCreated = await _userService.CreateUser(obj);

            if (isUserCreated)
            {
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
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserUpdateModel user)
        {
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
                return BadRequest("Old password is incorrect.");
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
                // Xử lý upload ảnh mới nếu file được chọn
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream())
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    return BadRequest(new ApiResponse<bool> { Success = false, ErrorMessage = uploadResult.Error.Message });
                }

                // Lưu ảnh mới và xóa ảnh cũ nếu có
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
