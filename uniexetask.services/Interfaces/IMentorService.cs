using uniexetask.core.Models;

namespace uniexetask.services.Interfaces
{
    public interface IMentorService
    {
        Task<Mentor?> GetMentorWithGroupAsync(int userId);
        Task<IEnumerable<Mentor>> GetMentorsAsync();
        Task<string?> GetMentorNameByGroupId(int groupId);
        Task<Mentor?> GetMentorByUserId(int userId);
        Task<Mentor?> GetMentorByEmail(string email);
        Task<Mentor?> GetMentorByGroupId(int groupId);
        Task<Mentor?> GetMentorById(int mentorId);
        Task<IEnumerable<Mentor>> GetMentorByCampusId(int campusId);
    }
}
