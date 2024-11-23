using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
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
        [HttpGet("getmilestonescore")]
        public async Task<IActionResult> GetMileStoneScore(int projectId, int mileStoneId)
        {
            ApiResponse<double> respone = new ApiResponse<double>();
            respone.Data = await _projectScoreService.GetMileStoneScore(projectId, mileStoneId);
            return Ok(respone);
        }
        [HttpPost]
        public async Task<IActionResult> AddProjectScore(AddProjectScoreModel addProjectScoreModel)
        {
            List<core.Models.ProjectScore> projectScoresToAdd = new List<core.Models.ProjectScore>();

            foreach(var projectScore in addProjectScoreModel.ProjectScores)
            {
                projectScoresToAdd.Add(new core.Models.ProjectScore
                {
                    ProjectId = addProjectScoreModel.ProjectId,
                    ScoredBy = addProjectScoreModel.ScoredBy,
                    ScoringDate = DateTime.Today,
                    CriteriaId = projectScore.CriteriaId,
                    Score = projectScore.Score,
                    Comment = addProjectScoreModel.Comment,
                });
            }


            bool result = await _projectScoreService.AddProjecScore(projectScoresToAdd);
            ApiResponse<AddProjectScoreModel> respone = new ApiResponse<AddProjectScoreModel>();
            respone.Success = result;
            respone.Data = addProjectScoreModel;
            return Ok(respone);
        }
    }
}
