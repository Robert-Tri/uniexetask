using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class MentorService: IMentorService
    {
        public IUnitOfWork _unitOfWork;
        public MentorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<Mentor?> GetMentorWithGroupAsync(int userId)
        {
            var mentor = await _unitOfWork.Mentors.GetMentorByUserId(userId);
            if (mentor == null)
            {
                return null;
            }

            var mentorWithGroup = await _unitOfWork.Mentors.GetMentorWithGroupAsync(mentor.MentorId);
            return mentorWithGroup;
        }
        public async Task<Mentor> GetMentorById(int id)
        {
            var mentor = await _unitOfWork.Mentors.GetByIDAsync(id);
            return mentor;
        }

        public async Task<string?> GetMentorNameByGroupId(int groupId)
        {
            var mentor = await _unitOfWork.Groups
                .GetAsync(g => g.GroupId == groupId && g.Mentors.Any(),
                          includeProperties: "Mentors.User"); 

            var mentorFullName = mentor?.FirstOrDefault()?.Mentors
                .FirstOrDefault()?.User?.FullName;

            return mentorFullName;
        }


        public async Task<IEnumerable<Mentor>> GetMentorsAsync()
        {
            var mentors = await _unitOfWork.Mentors.GetAsync();
            return mentors;
        }
        public async Task<Mentor?> GetMentorByUserId(int userId)
        {
            return await _unitOfWork.Mentors.GetMentorByUserId(userId);
        }

        public async Task<Mentor?> GetMentorByEmail(string email)
        {
            // Tìm kiếm User dựa trên email
            var user = (await _unitOfWork.Users.GetAsync(
                filter: u => u.Email == email && u.IsDeleted == false,
                includeProperties: "Mentors")).FirstOrDefault();

            if (user == null)
            {
                return null; // Trả về null nếu không tìm thấy user
            }

            // Trả về mentor của user, nếu có
            return user.Mentors.FirstOrDefault(); // Giả sử mỗi user có một mentor
        }
    }
}
