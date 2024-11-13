using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace uniexetask.api.Controllers
{
    [Authorize]
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
            ApiResponse<IEnumerable<Student>> response = new ApiResponse<IEnumerable<Student>>();
            try
            {
                var studentList = await _studentsService.GetAllStudent();
                if (studentList == null)
                {
                    response.Data = studentList;
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

    }
}
