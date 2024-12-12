using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Controllers
{
    [Route("api/criteria")]
    [ApiController]
    public class CriteriaController : ControllerBase
    {
        private readonly IProjectScoreService _projectScoreService;
        private readonly IMentorService _mentorService;
        private readonly IMapper _mapper;
        public CriteriaController(IProjectScoreService projectScoreService, IMentorService mentorService, IMapper mapper)
        {
            _projectScoreService = projectScoreService;
            _mentorService = mentorService;
            _mapper = mapper;
        }
    }

}
