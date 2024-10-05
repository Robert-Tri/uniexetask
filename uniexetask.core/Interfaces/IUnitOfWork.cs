using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        int Save();

        Task<int> SaveAsync();

        Task AddMentorToGroup(int groupId, int mentorId);
    }
}
