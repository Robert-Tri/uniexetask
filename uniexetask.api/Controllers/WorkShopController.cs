using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Extensions;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/workshop")]
    [ApiController]
    public class WorkShopController : ControllerBase
    {
        private readonly IWorkShopService _workShopService;
        private readonly IStudentService _studentService;
        private readonly IEmailService _emailService;
        public WorkShopController(IWorkShopService workShopService, IEmailService emailService, IStudentService studentService)
        {
            _workShopService = workShopService;
            _emailService = emailService;
            _studentService = studentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetWorkShops()
        {
            var workShops = await _workShopService.GetWorkShops();
            ApiResponse<IEnumerable<Workshop>> respone = new ApiResponse<IEnumerable<Workshop>>();
            respone.Data = workShops;
            return Ok(respone);
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPost]
        public async Task<IActionResult> CreateWorkShop(Workshop workshop)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            workshop.StartDate = TimeZoneInfo.ConvertTimeFromUtc(workshop.StartDate, timeZone);
            workshop.EndDate = TimeZoneInfo.ConvertTimeFromUtc(workshop.EndDate, timeZone);
            await _workShopService.CreateWorkShop(workshop);
            var workshopEmail = $@"
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
    <h2>Dear EXE Students,</h2>

    <p>We are excited to invite you to our workshop on <strong>{workshop.Name}</strong>. Below are the details of the event:</p>

    <p><strong>Workshop Information:</strong></p>
    <p><strong>Title:</strong> {workshop.Name}</p>
    <p><strong>Date:</strong> {workshop.StartDate.ToString("MMMM dd, yyyy")}</p>
    <p><strong>Time:</strong> {workshop.StartDate.ToString("hh:mm tt")} to {workshop.EndDate.ToString("hh:mm tt")}</p>
    <p><strong>Location:</strong> {workshop.Location}</p>
    <p><strong>Description:</strong> {workshop.Description}</p>

    <p>This is an automated email. Please do not reply to this email.</p>
    <p>Looking forward to your participation.</p>

    <p>Best regards,<br />
    [Your Name]<br />
    [Your Position]<br />
    [Your Contact Information]</p>
</body>
</html>
";
            var students = await _studentService.GetEligibleStudentsWithUser();

            foreach (var student in students)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _emailService.SendEmailAsync(student.User.Email, "Workshop Invitation", workshopEmail);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

            ApiResponse<Workshop> response = new ApiResponse<Workshop>
            {
                Data = workshop
            };
            return Ok(response);
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpPut]
        public async Task<IActionResult> UpdateWorkShop(Workshop workShop)
        {
            await _workShopService.UpdateWorkShop(workShop);
            ApiResponse<Workshop> respone = new ApiResponse<Workshop>();
            respone.Data = workShop;
            return Ok(respone);
        }
        [Authorize(Roles = nameof(EnumRole.Manager))]
        [HttpDelete("{workShopId}")]
        public IActionResult DeleteWorkShop(int workShopId)
        {
            _workShopService.DeleteWorkShop(workShopId);
            return NoContent();
        }
    }
}
