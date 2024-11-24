using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class GroupService : IGroupService
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

        public async Task<bool> UpdateGroupApproved(int groupId)
        {
                var group = await _unitOfWork.Groups.GetByIDAsync(groupId);
                if (group != null)
                {
                group.Status = "Approved";
                    _unitOfWork.Groups.Update(group);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            return false;
        }

        public async Task<IEnumerable<object>> GetApprovedGroupsAsync()
        {
            var groups = await _unitOfWork.Groups.GetApprovedGroupsWithGroupMembersAndStudent();

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
            // Bước 1: Lấy tất cả các group chưa có mentor
            var groups = await _unitOfWork.Groups.GetHasNoMentorGroupsWithGroupMembersAndStudent();

            // Bước 2: Đếm số lượng group trung bình cho mỗi mentor
            var mentorGroupCounts = new Dictionary<int, int>(); // Lưu số lượng nhóm đã được gán cho từng mentor

            var allMentors = await _unitOfWork.Mentors.GetAsync(); // Lấy tất cả các mentor từ cơ sở dữ liệu
            var totalGroups = groups.Count();
            var mentorCount = allMentors.Count();
            var averageGroupsPerMentor = Math.Max(totalGroups / mentorCount, 1); // Số nhóm trung bình mỗi mentor nên có

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
                    await AddMentorToGroup(group.GroupId, mentorToAdd.MentorId);
                    mentorGroupCounts[mentorToAdd.MentorId]++;
                }
            }
        }
        public async System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId)
        {
            var group = await _unitOfWork.Groups.GetByIDAsync(groupId);
            var mentor = await _unitOfWork.Mentors.GetByIDAsync(mentorId);
            if (group.HasMentor == true)
            {
                group.Mentors.Clear();
                group.Mentors.Add(mentor);
                _unitOfWork.Save();
            }
            else if (group.HasMentor == false)
            {
                if (group != null && mentor != null)
                {
                    group.Mentors.Add(mentor);
                    group.HasMentor = true;
                    _unitOfWork.Groups.Update(group);
                    _unitOfWork.Save();
                }
            }
        }

        public async Task<IEnumerable<Group>> GetAllGroup()
        {
            var groupList = await _unitOfWork.Groups.GetAsync(includeProperties: "Subject");
            return groupList;
        }

        public async Task<Group> GetGroupWithTopic(int groupId)
        {
            var group = await _unitOfWork.Groups.GetAsync(
                g => g.GroupId == groupId && g.IsDeleted == false,
                includeProperties: "Subject,RegTopicForms"
            );
            return group.FirstOrDefault();  
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

        public async Task<Group> GetGroupById(int id)
        {
            var group = await _unitOfWork.Groups.GetByIDAsync(id);
            return group;
        }


        public Task<IEnumerable<Group>> GetGroupsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Group?> GetGroupWithSubject(int groupId)
        {
            return await _unitOfWork.Groups.GetGroupWithSubjectAsync(groupId);
        }
        private List<int> DistributeStudents(int numStudents)
        {
            int minGroupSize = 4;
            int maxGroupSize = 6;

            List<int> groups = new List<int>();

            while (numStudents > 0)
            {
                int groupSize = Math.Min(maxGroupSize, numStudents);
                groups.Add(groupSize);
                numStudents -= groupSize;
            }

            for (int i = 0; i < groups.Count; i++)
            {
                while (groups[i] < minGroupSize)
                {
                    for (int j = 0; j < groups.Count; j++)
                    {
                        if (groups[j] > minGroupSize)
                        {
                            groups[j]--;
                            groups[i]++;
                            break;
                        }
                    }
                }
            }

            return groups;
        }

        private async System.Threading.Tasks.Task<HashSet<int>> UpdateEligibleGroup()
        {
            HashSet<int> studentIdSet = new HashSet<int>();
            var initializedGroup = await _unitOfWork.Groups.GetAsync(filter: g => g.Status == "Initialized");
            if (initializedGroup.Any())
            {
                foreach (var group in initializedGroup)
                {
                    var memberNumbers = await _unitOfWork.GroupMembers.GetAsync(filter: gm => gm.GroupId == group.GroupId);
                    foreach (var member in memberNumbers)
                    {
                        studentIdSet.Add(member.StudentId);
                    }

                    if (memberNumbers.Count() >= 4)
                    {
                        group.Status = "Eligible";
                        _unitOfWork.Groups.Update(group);
                    }
                }
                await _unitOfWork.SaveAsync();
            }
            return studentIdSet;
        }

        private async System.Threading.Tasks.Task AssignStudentsToGroups(HashSet<int> studentIdSet) 
        {
            var studentsWithoutGroup = (await _unitOfWork.Students.GetAsync(filter: s => !studentIdSet.Contains(s.StudentId) && s.IsCurrentPeriod == true)).ToList();
            var initializedGroup = (await _unitOfWork.Groups.GetAsync(filter: g => g.Status == "Initialized", includeProperties: "GroupMembers")).ToList();
            List<int> studentsInEachGroup = new List<int>();
            foreach (var group in initializedGroup)
            {
                var members = group.GroupMembers.Count();
                studentsInEachGroup.Add(members);
            }
            int totalStudent = studentsWithoutGroup.Count() + studentsInEachGroup.Sum();
            var distributedGroup = DistributeStudents(totalStudent);

            Dictionary<Group, int> groupDictionary = new Dictionary<Group, int>();

            foreach (var (group, index) in distributedGroup.Select((value, i) => (value, i)))
            {
                if (initializedGroup.Any())
                {
                    for (int i = 0; i < group - studentsInEachGroup[0]; i++)
                    {
                        var groupMember = new GroupMember
                        {
                            GroupId = initializedGroup[0].GroupId,
                            StudentId = studentsWithoutGroup[0].StudentId,
                            Role = "Member",
                        };
                        await _unitOfWork.GroupMembers.InsertAsync(groupMember);
                        initializedGroup[0].Status = "Eligible";
                        _unitOfWork.Groups.Update(initializedGroup[0]);
                        studentsWithoutGroup.RemoveAt(0);
                    }
                    initializedGroup.RemoveAt(0);
                }
                else
                {
                    var groupToAdd = new Group
                    {
                        GroupName = "Group " + (index + 1).ToString(),
                        SubjectId = 1,
                        HasMentor = false,
                        Status = "Eligible",
                    };
                    await _unitOfWork.Groups.InsertAsync(groupToAdd);
                    groupDictionary.Add(groupToAdd, group);
                }
            }
            await _unitOfWork.SaveAsync();

            foreach(var group in groupDictionary)
            {
                for(int i = 0; i < group.Value; i++)
                {
                    var groupMember = new GroupMember
                    {
                        GroupId = (await _unitOfWork.Groups.GetAsync(filter: g => g.GroupName == group.Key.GroupName)).FirstOrDefault().GroupId,
                        StudentId = studentsWithoutGroup[0].StudentId,
                        Role = i == 0 ? "Leader" : "Member",
                    };
                    await _unitOfWork.GroupMembers.InsertAsync(groupMember);
                }
            }
            await _unitOfWork.SaveAsync();

            /*foreach (var group in distributedGroup)
            {
                if (initializedGroup.Any())
                {
                    for (int i = 0; i < group - studentsInEachGroup[0]; i++)
                    {
                        var groupMember = new GroupMember
                        {
                            GroupId = initializedGroup[0].GroupId,
                            StudentId = studentsWithoutGroup[0].StudentId,
                            Role = "Member",
                        };
                        await _unitOfWork.GroupMembers.InsertAsync(groupMember);
                        initializedGroup[0].Status = "Eligible";
                        _unitOfWork.Groups.Update(initializedGroup[0]);
                        studentsWithoutGroup.RemoveAt(0);
                    }
                    initializedGroup.RemoveAt(0);
                }
                else
                {
                    var groupToAdd = new Group
                    {
                        GroupName = "Group " + group.ToString(),
                        SubjectId = 1,
                        HasMentor = false,
                        Status = "Eligible",
                    };
                    await _unitOfWork.Groups.InsertAsync(groupToAdd);
                    await _unitOfWork.SaveAsync();
                    for (int i = 0; i < group; i++)
                    {
                        var groupMember = new GroupMember
                        {
                            GroupId = (await _unitOfWork.Groups.GetAsync(filter: g => g.GroupName == groupToAdd.GroupName)).FirstOrDefault().GroupId,
                            StudentId = studentsWithoutGroup[0].StudentId,
                            Role = i == 0 ? "Leader" : "Member"
                        };
                        await _unitOfWork.GroupMembers.InsertAsync(groupMember);
                        studentsWithoutGroup.RemoveAt(0);
                    }
                }
            }
            await _unitOfWork.SaveAsync();*/
        }

        public async System.Threading.Tasks.Task UpdateAndAssignStudentsToGroups()
        {
            var studentIdSet = await UpdateEligibleGroup();
            await AssignStudentsToGroups(studentIdSet);
        }
    }
}
