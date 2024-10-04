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
    }
}
