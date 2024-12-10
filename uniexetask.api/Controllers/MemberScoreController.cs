using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
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
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        public MemberScoreController(IMemberScoreService memberScoreService, IMentorService mentorService,IMapper mapper) 
        {
            _memberScoreService = memberScoreService;
            _mentorService = mentorService;
            _mapper = mapper;
        }

        [HttpGet("getmemberscore")]
        public async Task<IActionResult> GetMemberScore(int projectId, int mileStoneId)
        {
            ApiResponse<MemberScoreResult> respone = new ApiResponse<MemberScoreResult>();
            respone.Data = await _memberScoreService.GetMemberScores(projectId, mileStoneId);
            return Ok(respone);
        }

        [HttpGet("gettotalmemberscore")]
        public async Task<IActionResult> GetTotalMemberScore(int projectId)
        {
            ApiResponse<TotalMemberScoreResult> respone = new ApiResponse<TotalMemberScoreResult>();
            respone.Data = await _memberScoreService.GetTotalMemberScore(projectId);
            return Ok(respone);
        }

        [Authorize(Roles = "Mentor")]
        [HttpPost("submitmemberscore")]
        public async Task<IActionResult> SubmitMemberScore(AddMemberScoreModel memberScore)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var mentor = await _mentorService.GetMentorByUserId(Int32.Parse(userId));
            List<MemberScore> memberScoreToAdd = new List<MemberScore>();
            foreach (var studentScore in memberScore.StudentScores) 
            {
                memberScoreToAdd.Add(new MemberScore
                {
                    ProjectId = memberScore.ProjectId,
                    MilestoneId = memberScore.MilestoneId,
                    ScoredBy = mentor.MentorId,
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
