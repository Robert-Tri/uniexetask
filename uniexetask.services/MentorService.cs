using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
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

        public async Task<Mentor?> GetMentorById(int id)
        {
            return await _unitOfWork.Mentors.GetByIDAsync(id);
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

        public async Task<Mentor?> GetMentorByGroupId(int groupId)
        {
            var group = await _unitOfWork.Groups
                .GetAsync(g => g.GroupId == groupId && g.Mentors.Any(),
                          includeProperties: "Mentors.User");

            
            var mentor = group?.FirstOrDefault()?.Mentors
                .FirstOrDefault();

            return mentor; 
        }


        public async Task<IEnumerable<Mentor>> GetMentorsAsync()
        {
            var mentors = await _unitOfWork.Mentors.GetAsync(includeProperties: "User");
            return mentors;
        }
        public async Task<Mentor?> GetMentorByUserId(int userId)
        {
            return await _unitOfWork.Mentors.GetMentorByUserId(userId);
        }

        public async Task<Mentor?> GetMentorByEmail(string email)
        {
            var user = (await _unitOfWork.Users.GetAsync(
                filter: u => u.Email == email && u.IsDeleted == false,
                includeProperties: "Mentors")).FirstOrDefault();

            if (user == null)
            {
                return null; 
            }

            return user.Mentors.FirstOrDefault(); 
        }

        public async Task<IEnumerable<Mentor>> GetMentorByCampusId(int campusId)
        {
            var userIds = (await _unitOfWork.Users.GetAsync(filter: u => u.CampusId == campusId && u.RoleId == (int)EnumRole.Mentor)).Select(u => u.UserId);
            var mentors = await _unitOfWork.Mentors.GetAsync(filter: m => userIds.Contains(m.UserId), includeProperties: "User");
            return mentors;
        }
        public async Task<bool> CreateMetor(Mentor mentor)
        {
            if(mentor != null)
            {
                await _unitOfWork.Mentors.InsertAsync(mentor);
                var result = _unitOfWork.Save();
                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<Mentor?> GetUserMentor(int userId)
        {
            return (await _unitOfWork.Mentors.GetAsync(filter: m => m.UserId == userId, includeProperties: "User")).FirstOrDefault();
        }

        public async Task<bool> UpdateMentor(Mentor mentor)
        {
            if(mentor != null)
            {
                var existedMentor = await _unitOfWork.Mentors.GetByIDAsync(mentor.UserId);
                if(existedMentor != null)
                {
                    existedMentor.Specialty = mentor.Specialty;
                    _unitOfWork.Mentors.Update(existedMentor);
                    int result = _unitOfWork.Save();
                    if(result > 0) 
                        return true;
                    return false;
                }
            }
            return false;
        }
    }
}
