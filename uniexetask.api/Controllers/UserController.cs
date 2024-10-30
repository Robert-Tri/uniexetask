using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    //[Authorize]
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
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
            var users = await _userService.GetUserById(userId);

            if (users != null)
            {
                return Ok(users);
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("ProfileUser")]
        public async Task<IActionResult> ProfileUser()
        {
            // Lấy userId từ claim trong JWT token
            var userIdString = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdString, out int userId)) // Chuyển userIdString thành int
            {
                // Gọi service để lấy thông tin user dựa trên userId
                var users = await _userService.GetUserByIdWithCampusAndRoleAndStudents(userId);

                if (users != null)
                {
                    return Ok(users); // Trả về thông tin user nếu tìm thấy
                }
                else
                {
                    return BadRequest("User not found"); // Trả về lỗi nếu không tìm thấy user
                }
            }
            else
            {
                return BadRequest("Invalid userId"); // Trả về lỗi nếu không lấy được userId
            }
        }


        [HttpPost]
        [Route("upload-excel")]
        public async Task<IActionResult> CreateUser(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                return BadRequest("File is not selected or is empty.");
            }

            var usersList = new List<UserModel>();

            // Đọc file Excel
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

                        int campusId;
                        switch (campusName)
                        {
                            case "FPT-HN":
                                campusId = 1;
                                break;
                            case "FPT-HCM":
                                campusId = 2;
                                break;
                            case "FPT-DN":
                                campusId = 3;
                                break;
                            default:
                                campusId = 0; 
                                break;
                        }

                        int roleId;
                        switch (roleName.ToLower()) 
                        {
                            case "admin":
                                roleId = 1;
                                break;
                            case "manager":
                                roleId = 2;
                                break;
                            case "student":
                                roleId = 3;
                                break;
                            case "mentor":
                                roleId = 4;
                                break;
                            case "sponsor":
                                roleId = 5;
                                break;
                            default:
                                roleId = 0;
                                break;
                        }

                        
                        string password = worksheet.Cells[row, 3].Text; 
                        int khoiNumber = int.Parse(khoiText.Substring(1)); 

                        if (khoiNumber >= 19)
                        {
                            
                            password = GenerateRandomPassword(6);
                        }

                        var user = new UserModel
                        {
                            FullName = worksheet.Cells[row, 2].Text,
                            Password = password,                       
                            Email = worksheet.Cells[row, 4].Text,
                            Phone = worksheet.Cells[row, 5].Text,
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
                var userEntity = _mapper.Map<User>(userModel);
                var isUserCreated = await _userService.CreateUser(userEntity);

                if (!isUserCreated)
                {
                    return BadRequest($"Failed to create user: {userModel.Email}");
                }
            }

            return Ok("All users were successfully created.");
        }

       
        private string GenerateRandomPassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
