using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    //[Authorize]
    [Route("api/projectscore")]
    [ApiController]
    public class ProjectScoreController : ControllerBase
    {
        private readonly IProjectScoreService _projectScoreService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        public ProjectScoreController(IProjectScoreService projectScoreService, IMentorService mentorService, IMapper mapper)
        {
            _projectScoreService = projectScoreService;
            _mentorService = mentorService;
            _mapper = mapper;
        }

        [HttpGet("getmilestonescore")]
        public async Task<IActionResult> GetMileStoneScore(int projectId, int mileStoneId)
        {
            ApiResponse<MilestoneScoreResult> respone = new ApiResponse<MilestoneScoreResult>();
            respone.Data = await _projectScoreService.GetMileStoneScore(projectId, mileStoneId);
            return Ok(respone);
        }

        [HttpGet("getprojectscore")]
        public async Task<IActionResult> GetProjectScore(int projectId)
        {
            ApiResponse<ProjectScoreResult> respone = new ApiResponse<ProjectScoreResult>();
            respone.Data = await _projectScoreService.GetTotalProjectScore(projectId);
            return Ok(respone);
        }

        [Authorize(Roles = "Mentor")]
        [HttpPost("submitprojectscore")]
        public async Task<IActionResult> SubmitProjectScore(AddProjectScoreModel addProjectScoreModel)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var mentor = await _mentorService.GetMentorByUserId(Int32.Parse(userId));
            List<core.Models.ProjectScore> projectScoresToAdd = new List<core.Models.ProjectScore>();

            foreach(var projectScore in addProjectScoreModel.ProjectScores)
            {
                projectScoresToAdd.Add(new core.Models.ProjectScore
                {
                    ProjectId = addProjectScoreModel.ProjectId,
                    ScoredBy = mentor.MentorId,
                    ScoringDate = DateTime.Today,
                    CriteriaId = projectScore.CriteriaId,
                    Score = projectScore.Score,
                    Comment = addProjectScoreModel.Comment,
                });
            }


            bool result = await _projectScoreService.SubmitProjecScore(projectScoresToAdd);
            ApiResponse<AddProjectScoreModel> respone = new ApiResponse<AddProjectScoreModel>();
            respone.Success = result;
            respone.Data = addProjectScoreModel;
            return Ok(respone);
        }
    }
}
