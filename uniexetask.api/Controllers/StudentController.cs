using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Extensions;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentsService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public StudentController(IStudentService studentsService, IUserService userService, IMapper mapper, IEmailService emailService)
        {
            _studentsService = studentsService;
            _userService = userService;
            _mapper = mapper;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentList()
        {
            ApiResponse<IEnumerable<Student>> response = new ApiResponse<IEnumerable<Student>>();
            try
            {
                var studentList = await _studentsService.GetAllStudent();

                if (studentList != null && studentList.Any()) 
                {
                    response.Data = studentList;
                    response.Success = true;
                    return Ok(response);
                }
                else
                {
                    response.Data = null;
                    response.Success = false;
                    response.ErrorMessage = "No students found.";
                    return NotFound(response); 
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }


        [HttpGet("bystudentcode")]
        public async Task<IActionResult> GetStudentByStudentCode(string studentCode)
        {
            var student = await _studentsService.GetStudentByCode(studentCode);

            if (student != null)
            {
                return Ok(student);
            }
            else
            {
                return BadRequest();
            }

        }
        [HttpGet("getuserrolestudent")]
        public async Task<IActionResult> GetUserRoleStudent(int userId)
        {
            var student = await _studentsService.GetUserRoleStudent(userId);
            ApiResponse<Student?> response = new ApiResponse<Student?>();
            response.Data = student;
            return Ok(response);
        }

        [HttpGet("byuserid")]
        public async Task<IActionResult> GetStudentByUserId(int userId)
        {
            ApiResponse<Student> response = new ApiResponse<Student>();
            try
            {
                var student = await _studentsService.GetStudentByUserId(userId);

                if (student != null)
                {
                    response.Data = student;
                    response.Success = true;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Student not found");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            ApiResponse<Student> response = new ApiResponse<Student>();
            try
            {
                var student = await _studentsService.GetStudentById(id);

                if (student != null)
                {
                    response.Data = student;
                    response.Success = true;
                    return Ok(response);
                }
                else
                {
                    throw new Exception("Student not found");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.ErrorMessage = ex.Message;
                return BadRequest(response);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateStudent(CreateStudentModel student)
        {
            bool isUserExisted = await _userService.CheckDuplicateUser(student.Email, student.Phone);
            if(isUserExisted)
                return Conflict("Email or phone has already been registered!");

            bool isStudentExisted = await _studentsService.CheckDuplicateStudentCode(student.StudentCode);
            if (isStudentExisted)
                return Conflict("Student code already exists!");

            string password = PasswordHasher.GenerateRandomPassword(10);
            var userModel = new UserModel
            {
                FullName = student.FullName,
                Password = PasswordHasher.HashPassword(password),
                Email = student.Email,
                Phone = student.Phone,
                CampusId = student.CampusId,
                IsDeleted = false,
                RoleId = 3,
                Avatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg"
            };
            var userEntity = _mapper.Map<User>(userModel);
            var isUserCreated = await _userService.CreateUser(userEntity);
            if (isUserCreated != null)
            {
                var studentModel = new StudentModel
                {
                    UserId = isUserCreated.UserId,
                    LecturerId = student.LectureId,
                    SubjectId = student.SubjectId,
                    StudentCode = student.StudentCode,
                    Major = student.Major,
                };
                var studentEntity = _mapper.Map<Student>(studentModel);
                var result = await _studentsService.CreateStudent(studentEntity);
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
    <h2>Dear {student.FullName},</h2>
    <p>We are sending you the following login account information:</p>
    <ul>
        <li><strong>Account:</strong> {student.Email}</li>
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
                _emailService.SendEmailAsync(student.Email, "Account UniEXETask", userEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                ApiResponse<Student> respone = new ApiResponse<Student>();
                respone.Data = studentEntity;
                respone.Success = result;
                return Ok(respone);
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateStudentByUserId(UpdateStudentModel studentModel)
        {
            var isExistedUser = await _userService.CheckDuplicateUserForUpdate(studentModel.UserId, studentModel.Email, studentModel.Phone);
            if(isExistedUser)
                return Conflict("Email or phone has already been registered!");
            var isExistedStudent = await _studentsService.CheckDuplicateStudenCodeForUpdate(studentModel.UserId, studentModel.StudentCode);
            if (isExistedStudent)
                return Conflict("Student code already exists!");

            var resultUpdateUser = await _userService.UpdateUser(new core.Models.User { UserId = studentModel.UserId, Email = studentModel.Email, Phone = studentModel.Phone, FullName = studentModel.FullName, CampusId = studentModel.CampusId });
            if (resultUpdateUser)
            {
                var resultUpdateStudent = await _studentsService.UpdateStudent(new Student { StudentCode = studentModel.StudentCode, LecturerId = studentModel.LectureId, Major = studentModel.Major, SubjectId = studentModel.SubjectId });
                ApiResponse<bool> respone = new ApiResponse<bool>();
                respone.Success = true;
                respone.Data = resultUpdateStudent;
                return Ok(respone);
            }
            return BadRequest();
        }
    }
}
