using AutoMapper;
using uniexetask.core.Models;
using uniexetask.shared.Models.Request;

namespace Unitask.shared.Extensions
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
            CreateMap<ReqTopicModel, RegTopicForm>().ReverseMap();
            CreateMap<TopicModel, Topic>().ReverseMap();
            CreateMap<TopicForMentorModel, TopicForMentor>().ReverseMap();
            CreateMap<GroupModel, Group>().ReverseMap();
            CreateMap<SubjectModel, Subject>().ReverseMap();
            CreateMap<StudentModel, Student>().ReverseMap();
            CreateMap<FeatureModel, Feature>().ReverseMap();
            CreateMap<PermissionModel, Permission>().ReverseMap();
            CreateMap<CreateTaskModel, uniexetask.core.Models.Task>().ReverseMap();
            CreateMap<UpdateTaskModel, uniexetask.core.Models.Task>().ReverseMap();
            CreateMap<GroupInviteModel, GroupInvite>().ReverseMap();
            CreateMap<NotificationModel, Notification>().ReverseMap();
            CreateMap<AddMemberScoreModel, MemberScore>().ReverseMap();
            CreateMap<AddProjectScoreModel, uniexetask.core.Models.ProjectScore>().ReverseMap();
            CreateMap<SubjectModel, Subject>().ReverseMap();
            CreateMap<ProjectModel, Project>().ReverseMap();
            CreateMap<ProjectProgressModel, ProjectProgress>().ReverseMap();
            CreateMap<MeetingScheduleModel, MeetingSchedule>().ReverseMap();

        }
    }
}
