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
        public IMentorRepository Mentors { get; }

        public UnitOfWork(UniExetaskContext dbContext,
                            IUserRepository userRepository,
                            IRefreshTokenRepository refreshTokens,
                            IRoleRepository roles,
                            IFeatureRepository features,
                            IPermissionRepository permissions,
                            ICampusRepository campusRepository,
                            IProjectRepository projects,
                            IGroupRepository groups,
                            IGroupMemberRepository groupMembers,
                            IMentorRepository mentors)
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
            GroupMembers = groupMembers;
            Mentors = mentors;
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
        public async Task AddMentorToGroup(int groupId, int mentorId)
        {
            var group = await _dbContext.Groups.FindAsync(groupId);
            var mentor = await _dbContext.Mentors.FindAsync(mentorId);
            if (group.Status == "Status 2")
            {
                group.Mentors.Clear();
                group.Mentors.Add(mentor);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                if (group != null && mentor != null)
                {
                    group.Mentors.Add(mentor);
                    group.Status = "Status 2";
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
