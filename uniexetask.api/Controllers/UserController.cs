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
        private readonly IStudentService _studentsService;
        private readonly IUserService _userService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly Cloudinary _cloudinary;
        public UserController(IUserService userService, IMentorService mentorService, IMapper mapper, IEmailService emailService, IStudentService studentsService)
        {
            _studentsService = studentsService;
            _mentorService = mentorService;
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
         
            var excelList = new List<ExcelModel>();
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
                        var roleName = worksheet.Cells[row, 7].Text;
                        var khoiText = worksheet.Cells[row, 8].Text;
                        var subjectId = worksheet.Cells[row, 9].Text;
                        string studentcode = worksheet.Cells[row, 10].Text;
                        string major = worksheet.Cells[row,11].Text;
                        //var lecturer = worksheet.Cells[row, 12].Text;

                        string lecturer = worksheet.Cells[row, 12].Text;

                        var lecturerUser = await _mentorService.GetMentorByEmail(lecturer);

                        int campusId = campusName switch
                        {
                            "FPT-HN" => 1,
                            "FPT-HCM" => 2,
                            "FPT-DN" => 3,
                            _ => 0
                        };

                        int subject = subjectId switch
                        {
                            "EXE101" => 1,
                            "EXE201" => 2,
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

                        string fullname = worksheet.Cells[row, 2].Text;
                        string password = worksheet.Cells[row, 3].Text;
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

                        var excel = new ExcelModel
                        {
                            FullName = fullname,
                            Password = password,
                            Email = emailUser,
                            Phone = phoneUser,
                            CampusId = campusId,
                            IsDeleted = false,
                            RoleId = roleId,
                            LecturerId = lecturerUser.MentorId,
                            StudentCode = studentcode,
                            Major = major,
                            SubjectId = subject,
                            IsCurrentPeriod = true
                        };

                        excelList.Add(excel);


                    }
                }
            }

            var emailTasks = new List<System.Threading.Tasks.Task>();

            foreach (var excelModel in excelList)
            {
                string rawPassword = excelModel.Password;

                excelModel.Password = PasswordHasher.HashPassword(excelModel.Password);

                var userModel = new UserModel
                {
                    FullName = excelModel.FullName,
                    Password = excelModel.Password,
                    Email = excelModel.Email,
                    Phone = excelModel.Phone,
                    CampusId = excelModel.CampusId,
                    IsDeleted = false,
                    RoleId = excelModel.RoleId
                };

                var userEntity = _mapper.Map<User>(userModel);

                var isUserCreated = await _userService.CreateUserExcel(userEntity);

                var studentModel = new StudentModel
                {
                    UserId = isUserCreated.UserId,
                    LecturerId = excelModel.LecturerId,
                    StudentCode = excelModel.StudentCode,
                    Major = excelModel.Major,
                    SubjectId = excelModel.SubjectId,
                    IsCurrentPeriod = true
                };

                var studentEntity = _mapper.Map<Student>(studentModel);

                var isStudentCreated = await _studentsService.CreateStudent(studentEntity);


                if (isUserCreated == null)
                {
                    var response = new ApiResponse<bool>
                    {
                        Success = false,
                        ErrorMessage = $"Failed to create user: {userModel.Email}"
                    };
                    return BadRequest(response);
                }

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

                var emailTask = _emailService.SendEmailAsync(userModel.Email, "Users Invitation", userEmail);
                emailTasks.Add(emailTask);
            }

            await System.Threading.Tasks.Task.WhenAll(emailTasks);

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
