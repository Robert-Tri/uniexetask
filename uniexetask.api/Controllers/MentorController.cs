using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Extensions;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/mentor")]
    [ApiController]
    public class MentorController : ControllerBase
    {
        private readonly IMentorService _mentorService;
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IConfigSystemService _configSystemService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public MentorController(IMentorService mentorService, IGroupService groupService, IConfigSystemService configSystemService, IUserService userService, IMapper mapper, IEmailService emailService)
        {
            _mentorService = mentorService;
            _groupService = groupService;
            _configSystemService = configSystemService;
            _userService = userService;
            _mapper = mapper;
            _emailService = emailService;
        }
        [HttpGet]
        public async Task<IActionResult> GetMentors()
        {
            ApiResponse<IEnumerable<Mentor>> respone = new ApiResponse<IEnumerable<Mentor>>();
            respone.Data = await _mentorService.GetMentorsAsync();
            return Ok(respone);
        }
        [HttpGet("getmentorsbycampusid")]
        public async Task<IActionResult> GetMentorsByCampusId(int campusId)
        {
            ApiResponse<IEnumerable<Mentor>> response = new ApiResponse<IEnumerable<Mentor>>();
            response.Data = await _mentorService.GetMentorByCampusId(campusId);
            return Ok(response);
        }
        [HttpPost("assignmentortogroup")]
        public async Task<IActionResult> AssignMentorToGroup(int groupId, int mentorId)
        {
            var configSystem = await _configSystemService.GetConfigSystemById((int)ConfigSystemName.EDIT_MENTOR);
            if (DateTime.Today > configSystem.StartDate)
                return StatusCode(403, "Editing mentors is not allowed after the specified date.");
            await _groupService.AddMentorToGroup(groupId, mentorId);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> CreateMentor(CreateMentorModel mentor)
        {
            var isUserExisted = await _userService.CheckDuplicateUser(mentor.Email, mentor.Phone);
            if(isUserExisted)
                return Conflict("Email or phone has already been registered!");
            string password = PasswordHasher.GenerateRandomPassword(10);
            var userModel = new UserModel
            {
                FullName = mentor.FullName,
                Password = PasswordHasher.HashPassword(password),
                Email = mentor.Email,
                Phone = mentor.Phone,
                CampusId = mentor.CampusId,
                IsDeleted = false,
                RoleId = 4,
                Avatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg"
            };
            var userEntity = _mapper.Map<User>(userModel);
            var isUserCreated = await _userService.CreateUser(userEntity);
            if(isUserCreated != null)
            {
                var mentorModel = new Mentor
                {
                    UserId = isUserCreated.UserId,
                    Specialty = mentor.Specialty
                };
                var result = await _mentorService.CreateMetor(mentorModel);
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
    <h2>Dear {mentor.FullName},</h2>
    <p>We are sending you the following login account information:</p>
    <ul>
        <li><strong>Account:</strong> {mentor.Email}</li>
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
                _emailService.SendEmailAsync(mentor.Email, "Account UniEXETask", userEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                ApiResponse<Mentor> respone = new ApiResponse<Mentor>();
                respone.Data = mentorModel;
                respone.Success = result;
                return Ok(respone);
            }
            return BadRequest();
        }
    }
}
