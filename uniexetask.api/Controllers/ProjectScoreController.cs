using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/projectscore")]
    [ApiController]
    public class ProjectScoreController : ControllerBase
    {
        private readonly IProjectScoreService _projectScoreService;
        private readonly IMapper _mapper;
        public ProjectScoreController(IProjectScoreService projectScoreService, IMapper mapper)
        {
            _projectScoreService = projectScoreService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> AddProjectScore(AddProjectScoreModel addProjectScoreModel)
        {
            bool result = await _projectScoreService.AddProjecScore(_mapper.Map<ProjectScore>(addProjectScoreModel));
            ApiResponse<AddProjectScoreModel> respone = new ApiResponse<AddProjectScoreModel>();
            respone.Success = result;
            respone.Data = addProjectScoreModel;
            return Ok(respone);
        }
    }
}
