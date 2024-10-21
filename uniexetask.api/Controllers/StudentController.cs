using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentsService;

        public StudentController(IStudentService studentsService)
        {
            _studentsService = studentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentList()
        {
            var studentList = await _studentsService.GetAllStudent();
            if (studentList == null)
            {
                return NotFound();
            }
            ApiResponse<IEnumerable<Student>> response = new ApiResponse<IEnumerable<Student>>();
            response.Data = studentList;
            return Ok(response);
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

        [HttpGet("byuserid")]
        public async Task<IActionResult> GetStudentByUserId(int userId)
        {
            var student = await _studentsService.GetStudentByUserId(userId);

            if (student != null)
            {
                return Ok(student);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentsService.GetStudentById(id);

            if (student != null)
            {
                ApiResponse<Student> response = new ApiResponse<Student>
                {
                    Data = student
                };
                return Ok(response);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
