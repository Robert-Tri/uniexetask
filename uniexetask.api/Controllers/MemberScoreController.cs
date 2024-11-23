using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using uniexetask.api.Models.Request;
using uniexetask.api.Models.Response;
using uniexetask.core.Models;
using uniexetask.services;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/memberscore")]
    [ApiController]
    public class MemberScoreController : ControllerBase
    {
        private readonly IMemberScoreService _memberScoreService;
        private readonly IMapper _mapper;
        public MemberScoreController(IMemberScoreService memberScoreService, IMapper mapper) 
        {
            _memberScoreService = memberScoreService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> AddMemberScore(AddMemberScoreModel memberScore)
        {
            List<MemberScore> memberScoreToAdd = new List<MemberScore>();
            foreach (var studentScore in memberScore.StudentScores) 
            {
                memberScoreToAdd.Add(new MemberScore
                {
                    ProjectId = memberScore.ProjectId,
                    MilestoneId = memberScore.MilestoneId,
                    ScoredBy = memberScore.ScoredBy,
                    ScoringDate = DateTime.Today,
                    StudentId = studentScore.StudentId,
                    Score = studentScore.Score,
                    Comment = studentScore.Comment,
                });
            }
            bool result = await _memberScoreService.AddMemberScore(memberScoreToAdd);
            ApiResponse<AddMemberScoreModel> respone = new ApiResponse<AddMemberScoreModel>();
            respone.Success = result;
            respone.Data = memberScore;
            return Ok(respone);
        }
    }
}
