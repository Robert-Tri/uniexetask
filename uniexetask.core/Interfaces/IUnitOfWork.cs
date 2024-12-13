namespace uniexetask.core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICampusRepository Campus { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IRoleRepository Roles { get; }
        IFeatureRepository Features { get; }
        IPermissionRepository Permissions { get; }
        IProjectRepository Projects { get; }
        IGroupRepository Groups { get; }
        IGroupMemberRepository GroupMembers { get; }
        IStudentRepository Students { get; }
        IMentorRepository Mentors { get; }
        ITopicRepository Topics { get; }
        IChatGroupRepository ChatGroups { get; }
        IChatMessageRepository ChatMessages { get; }
        IWorkShopRepository WorkShops { get; }
        ITimeLineRepository TimeLines { get; }
        ITaskRepository Tasks { get; }
        ITaskAssignRepository TaskAssigns { get; }
        IReqMemberRepository ReqMembers { get; }
        IDocumentRepository Documents { get; }
        IGroupInviteRepository GroupInvites { get; }
        INotificationRepository Notifications { get; }
        IProjectProgressRepository ProjectProgresses { get; }
        ITaskProgressRepository TaskProgresses { get; }
        IMemberScoreRepository MemberScores { get; }
        IMilestoneRepository Milestones { get; }
        IProjectScoreRepository ProjectScores { get; }
        ISubjectRepository Subjects { get; }

        ITaskDetailRepository TaskDetails { get; }
        IReqTopicRepsitory ReqTopic { get; }
        IMeetingScheduleRepository MeetingSchedules { get; }
        IConfigSystemRepository ConfigSystems { get; }
        ITopicForMentorRepository TopicForMentor { get; }
        ICriterionRepository Criterion { get; }

        int Save();

        Task<int> SaveAsync();

        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
