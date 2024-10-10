using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;

namespace uniexetask.infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly UniExetaskContext _dbContext;
        public IUserRepository Users { get; }
        public IRefreshTokenRepository RefreshTokens { get; }
        public IRoleRepository Roles { get; }
        public IFeatureRepository Features { get; }
        public IPermissionRepository Permissions { get; }
        public ICampusRepository Campus { get; }
        public IProjectRepository Projects { get; }
        public IGroupRepository Groups { get; }
        public IGroupMemberRepository GroupMembers { get; }
        public IStudentRepository Students { get; }
        public IMentorRepository Mentors { get; }
        public ITopicRepository Topics { get; }
        public IChatGroupRepository ChatGroups { get; }
        public IChatMessageRepository ChatMessages { get; }
        public IWorkShopRepository WorkShops { get; }
        public ITimeLineRepository TimeLines { get; }
        public ITaskRepository Tasks { get; }
        public ITaskAssignRepository TaskAssigns { get; }

        public UnitOfWork(UniExetaskContext dbContext,
                            IUserRepository userRepository,
                            IRefreshTokenRepository refreshTokens,
                            IRoleRepository roles,
                            IFeatureRepository features,
                            IPermissionRepository permissions,
                            ICampusRepository campusRepository,
                            IProjectRepository projects,
                            IGroupRepository groups,
                            ITopicRepository topics,
                            IGroupMemberRepository groupMembers,
                            IStudentRepository students,
                            IMentorRepository mentors,
                            IChatGroupRepository chatGroups,
                            IChatMessageRepository chatMessages
                            IWorkShopRepository workshops,
                            ITimeLineRepository timelines,
                            ITaskRepository tasks,
                            ITaskAssignRepository taskAssigns)
        {
            _dbContext = dbContext;
            Users = userRepository;
            Campus = campusRepository;
            RefreshTokens = refreshTokens;
            Roles = roles;
            Features = features;
            Permissions = permissions;
            Projects = projects;
            Groups = groups;
            Students = students;
            GroupMembers = groupMembers;
            Mentors = mentors;
            Topics = topics;
            ChatGroups = chatGroups;
            ChatMessages = chatMessages;
            WorkShops = workshops;
            TimeLines = timelines;
            Tasks = tasks;
            TaskAssigns = taskAssigns;
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        public async Task<int> SaveAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
