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
            CreateMap<CreateUserModel, User>().ReverseMap();
            CreateMap<UserUpdateModel, User>().ReverseMap();
            CreateMap<GroupMemberModel, GroupMember>().ReverseMap();
            CreateMap<RegMemberFormModel, RegMemberForm>().ReverseMap();
            CreateMap<GroupModel, Group>().ReverseMap();
            CreateMap<SubjectModel, Subject>().ReverseMap();
            CreateMap<FeatureModel, Feature>().ReverseMap();
            CreateMap<PermissionModel, Permission>().ReverseMap();
            CreateMap<CreateTaskModel, uniexetask.core.Models.Task>().ReverseMap();

        }
    }
}
