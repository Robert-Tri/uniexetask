using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.infrastructure.Repositories;

namespace uniexetask.infrastructure.ServiceExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UniExetaskContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICampusRepository, CampusRepsitory>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IFeatureRepository, FeatureRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupMemberRepository, GroupMemberRepsitory>();
            services.AddScoped<IMentorRepository, MentorRepository>();
            services.AddScoped<IStudentRepository, StudentRepsitory>();
            services.AddScoped<ITopicRepository, TopicRepository>();
            services.AddScoped<IChatGroupRepository, ChatGroupRepository>();
            services.AddScoped<IChatMessageRepository, ChatMessageRepository>();
            services.AddScoped<IWorkShopRepository, WorkShopRepository>();
            services.AddScoped<ITimeLineRepository, TimeLineRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskAssignRepository, TaskAssignRepository>();
            services.AddScoped<IReqMemberRepository, ReqMemberRepsitory>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<IProjectProgressRepository, ProjectProgressRepository>();
            services.AddScoped<ITaskProgressRepository, TaskProgressRepository>();
            services.AddScoped<IUsagePlanRepository, UsagePlanRepository>();
            services.AddScoped<IMemberScoreRepository, MemberScoreRepository>();
            services.AddScoped<IMilestoneRepository, MilestoneRepository>();
            services.AddScoped<IGroupInviteRepository, GroupInviteRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IProjectScoreRepository, ProjectScoreRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ITaskDetailRepository, TaskDetailRepository>();
            services.AddScoped<IReqTopicRepsitory, ReqTopicRepsitory>();
            services.AddScoped<IMeetingScheduleRepository, MeetingScheduleRepository>();
            services.AddScoped<ITopicForMentorRepository, TopicForMentorRepsitory>();

            return services;
        }
    }
}
