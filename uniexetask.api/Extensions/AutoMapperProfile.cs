using AutoMapper;
using uniexetask.core.Models;
using uniexetask.api.Models.Request;

namespace Unitask.Api.Extensions
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserModel, User>().ReverseMap();

        }
    }
}
