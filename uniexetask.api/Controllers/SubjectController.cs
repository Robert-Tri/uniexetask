using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Authorize]
    [Route("api/subject")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;
        public SubjectController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetSubject()
        {
            var subject = await _subjectService.GetSubjects();
            ApiResponse<IEnumerable<SubjectModel>> respone = new ApiResponse<IEnumerable<SubjectModel>>();
            respone.Data = _mapper.Map<IEnumerable<SubjectModel>>(subject);
            return Ok(respone);
        }
    }
}
