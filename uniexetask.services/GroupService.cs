using Microsoft.AspNetCore.Mvc;
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
    public class GroupService: IGroupService
    {
        public IUnitOfWork _unitOfWork;
        public GroupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Group>> GetGroupAndSubject()
        {
            var groups = await _unitOfWork.Groups.GetAsync(includeProperties: "Subject");
            return groups;
        }

        public async Task<IEnumerable<object>> GetApprovedGroupsAsync()
        {
            var groups = await _unitOfWork.Groups.GetAsync(
                filter: g => g.HasMentor == false,
                includeProperties: "GroupMembers.Student"
            );

            var result = groups.Select(g => new
            {
                g.GroupId,
                g.GroupName, 
                g.Status,
                GroupMembers = g.GroupMembers.Select(m => new
                {
                    m.StudentId,
                    m.Role,
                    Student = new
                    {
                        m.Student.StudentCode,
                        m.Student.Major,
                        m.Student.LecturerId,
                    }
                })
            });

            return result;
        }

        public async System.Threading.Tasks.Task AddMentorToGroupAutomatically()
        {
            // Bước 1: Lấy tất cả các group có Status 1
            var groups = (IEnumerable<dynamic>)(await GetApprovedGroupsAsync());

            // Bước 2: Đếm số lượng group trung bình cho mỗi mentor
            var mentorGroupCounts = new Dictionary<int, int>(); // Lưu số lượng nhóm đã được gán cho từng mentor

            var allMentors = await _unitOfWork.Mentors.GetAsync(); // Lấy tất cả các mentor từ cơ sở dữ liệu
            var totalGroups = groups.Count();
            var mentorCount = allMentors.Count();
            var averageGroupsPerMentor = totalGroups / mentorCount; // Số nhóm trung bình mỗi mentor nên có

            foreach (var mentor in allMentors)
            {
                mentorGroupCounts[mentor.MentorId] = 0; // Bắt đầu với 0 nhóm cho mỗi mentor
            }

            // Bước 3: Duyệt qua từng group và tìm lecturerId phổ biến nhất
            foreach (var group in groups)
            {
                // Tạo dictionary để đếm số lần xuất hiện của mỗi LecturerId trong nhóm
                var lecturerCount = new Dictionary<int, int>();

                foreach (var member in group.GroupMembers)
                {
                    var lecturerId = member.Student.LecturerId;

                    if (lecturerCount.ContainsKey(lecturerId))
                    {
                        lecturerCount[lecturerId]++;
                    }
                    else
                    {
                        lecturerCount[lecturerId] = 1;
                    }
                }

                // Sắp xếp các LecturerId theo thứ tự phổ biến giảm dần
                var sortedLecturerIds = lecturerCount.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();

                Mentor? mentorToAdd = null;

                // Bước 4: Chọn mentor phổ biến nhất nhưng không vượt quá số nhóm trung bình
                foreach (var lecturerId in sortedLecturerIds)
                {
                    mentorToAdd = allMentors.FirstOrDefault(m => m.MentorId == lecturerId);

                    if (mentorToAdd != null && mentorGroupCounts[mentorToAdd.MentorId] < averageGroupsPerMentor)
                    {
                        // Mentor chưa vượt quá giới hạn trung bình, có thể gán vào group
                        break;
                    }

                    // Nếu mentor phổ biến nhất đã vượt quá số nhóm trung bình, chuyển qua mentor ít phổ biến hơn
                    mentorToAdd = null;
                }

                // Bước 5: Nếu tất cả các mentor phổ biến đều vượt quá số nhóm trung bình, chọn mentor có ít nhóm nhất
                if (mentorToAdd == null)
                {
                    mentorToAdd = mentorGroupCounts
                        .OrderBy(m => m.Value) // Sắp xếp mentor theo số lượng nhóm đã gán
                        .Select(m => allMentors.FirstOrDefault(mentor => mentor.MentorId == m.Key))
                        .FirstOrDefault();
                }

                // Bước 6: Gán mentor vào group
                if (mentorToAdd != null)
                {
                    await _unitOfWork.AddMentorToGroup(group.GroupId, mentorToAdd.MentorId);
                    mentorGroupCounts[mentorToAdd.MentorId]++; // Cập nhật số nhóm mà mentor này đã được gán
                }
            }
        }

        public async Task<IEnumerable<Group>> GetAllGroup()
        {
            var groupList = await _unitOfWork.Groups.GetAsync(includeProperties: "Subject");
            return groupList;
        }

        public async Task<bool> CreateGroup(Group group)
        {
            if (group != null)
            {
                await _unitOfWork.Groups.InsertAsync(group);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        /*        public async Task<Group?> GetGroupWithProjectAsync(int groupId)
                {
                    var group = await _unitOfWork.Groups.GetGroupWithProjectAsync(groupId);
                    return group;
                }*/

        public async Task<Group> GetGroupById(int id)
        {
            var group = await _unitOfWork.Groups.GetByIDAsync(id);
            return group;
        }

        public async System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId)
        {
            await _unitOfWork.AddMentorToGroup(groupId, mentorId);
        }

        public Task<IEnumerable<Group>> GetGroupsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
