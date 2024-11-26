using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
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
            var groups = await _unitOfWork.Groups.GetAsync(includeProperties: "Subject", filter: q => q.IsDeleted == false);
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
            var groups = await _unitOfWork.Groups.GetHasNoMentorGroupsWithGroupMembersAndStudent();

            var groupsByCampus = new Dictionary<int, List<Group>>();

            foreach (var group in groups)
            {
                var campusId = group.GroupMembers.FirstOrDefault()?.Student?.User?.CampusId;

                if (campusId.HasValue)
                {
                    if (!groupsByCampus.ContainsKey(campusId.Value))
                    {
                        groupsByCampus[campusId.Value] = new List<Group>();
                    }
                    groupsByCampus[campusId.Value].Add(group);
                }
            }

            foreach (var campusGroups in groupsByCampus)
            {
                int campusId = campusGroups.Key;

                var allMentors = (await _unitOfWork.Mentors.GetMentorsWithCampus())
                    .Where(m => m.User?.CampusId == campusId)
                    .ToList();

                if (!allMentors.Any()) continue;

                var mentorGroupCounts = allMentors.ToDictionary(m => m.MentorId, _ => 0);

                var totalGroups = campusGroups.Value.Count;
                var mentorCount = allMentors.Count;
                var averageGroupsPerMentor = Math.Max(totalGroups / mentorCount, 1);

                foreach (var group in campusGroups.Value)
                {
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

                    var sortedLecturerIds = lecturerCount
                        .OrderByDescending(x => x.Value)
                        .Select(x => x.Key)
                        .ToList();

                    Mentor? mentorToAdd = null;

                    foreach (var lecturerId in sortedLecturerIds)
                    {
                        mentorToAdd = allMentors.FirstOrDefault(m => m.MentorId == lecturerId);

                        if (mentorToAdd != null && mentorGroupCounts[mentorToAdd.MentorId] < averageGroupsPerMentor)
                        {
                            break;
                        }

                        mentorToAdd = null;
                    }

                    if (mentorToAdd == null)
                    {
                        mentorToAdd = mentorGroupCounts
                            .OrderBy(m => m.Value)
                            .Select(m => allMentors.FirstOrDefault(mentor => mentor.MentorId == m.Key))
                            .FirstOrDefault();
                    }

                    if (mentorToAdd != null)
                    {
                        await AddMentorToGroup(group.GroupId, mentorToAdd.MentorId);
                        mentorGroupCounts[mentorToAdd.MentorId]++;
                    }
                }
            }
        }

        public async System.Threading.Tasks.Task AddMentorToGroup(int groupId, int mentorId)
        {
            var group = await _unitOfWork.Groups.GetByIDAsync(groupId);
            var mentor = await _unitOfWork.Mentors.GetByIDAsync(mentorId);
            var chatGroup = await _unitOfWork.ChatGroups.GetChatGroupByGroupId(groupId);
            if (group.HasMentor == true)
            {
                group.Mentors.Clear();
                group.Mentors.Add(mentor);
                _unitOfWork.Save();
            }
            else if (group.HasMentor == false)
            {
                if (group != null && mentor != null && chatGroup != null)
                {
                    var chatGroupWithUsers = await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroup.ChatGroupId);
                    if (chatGroupWithUsers != null) 
                    {
                        bool isMentorInGroup = chatGroupWithUsers.Users.Any(u => u.UserId == mentor.UserId);
                        if (!isMentorInGroup)
                        {
                            var user = await _unitOfWork.Users.GetByIDAsync(mentor.UserId);
                            if (user != null)
                            {
                                chatGroup.Users.Add(user);
                                _unitOfWork.ChatGroups.Update(chatGroup);
                                _unitOfWork.Save();
                            }
                        }
                    }

                    _unitOfWork.ChatGroups.Update(chatGroup);
                    _unitOfWork.Save();
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
        private List<int> DistributeStudents(int numStudents, SubjectType subjectType)
        {
            int minGroupSize = 0;
            int maxGroupSize = 0;

            if (SubjectType.EXE101 == subjectType)
            {
                minGroupSize = 4;
                maxGroupSize = 6;
            }
            else if (SubjectType.EXE201 == subjectType)
            {
                minGroupSize = 8;
                maxGroupSize = 10;
            }

            if(numStudents < minGroupSize)
            {
                return new List<int>();
            }

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

        private async Task<HashSet<int>> UpdateEligibleGroup(SubjectType subjectType)
        {
            int minMembers = 0;
            if (1 == (int)subjectType)
                minMembers = 4;
            else if (2 == (int)subjectType)
                minMembers = 6;
            HashSet<int> studentIdSet = new HashSet<int>();
            var initializedGroup = await _unitOfWork.Groups.GetAsync(filter: g => g.IsDeleted == false && g.Status == "Initialized" && g.SubjectId == (int)subjectType);
            if (initializedGroup.Any())
            {
                foreach (var group in initializedGroup)
                {
                    var memberNumbers = await _unitOfWork.GroupMembers.GetAsync(filter: gm => gm.GroupId == group.GroupId);

                    if (memberNumbers.Count() >= minMembers)
                    {
                        foreach (var member in memberNumbers)
                        {
                            studentIdSet.Add(member.StudentId);
                        }
                        group.Status = "Eligible";
                        _unitOfWork.Groups.Update(group);
                    }
                    else
                    {
                        group.IsDeleted = true;
                        await _unitOfWork.GroupMembers.DeleteGroupMembers(group.GroupId);
                        _unitOfWork.Groups.Update(group);
                    }
                }
                await _unitOfWork.SaveAsync();
            }
            return studentIdSet;
        }

        private async System.Threading.Tasks.Task AssignStudentsToGroups(HashSet<int> studentIdSet, SubjectType subjectType)
        {

            var groupMemberStudentIds = (await _unitOfWork.GroupMembers.GetAsync()).Select(gm => gm.StudentId).ToHashSet();

            var studentsWithoutGroup = (await _unitOfWork.Students.GetAsync(filter: s =>
                (!groupMemberStudentIds.Contains(s.StudentId) && s.IsCurrentPeriod && s.SubjectId == (int)subjectType))).ToList();

            if (!studentsWithoutGroup.Any())
            {
                return;
            }
            var userIds = studentsWithoutGroup.Select(s => s.UserId).ToHashSet();
            var users = (await _unitOfWork.Users.GetAsync(filter: u => userIds.Contains(u.UserId))).ToList();

            var userCampusMap = users.ToDictionary(u => u.UserId, u => u.CampusId);

            var campusSubjectStudents = new Dictionary<(int CampusId, int SubjectId), List<Student>>();

            foreach (var student in studentsWithoutGroup)
            {
                if (userCampusMap.TryGetValue(student.UserId, out var campusId))
                {
                    var key = (CampusId: campusId, SubjectId: student.SubjectId);
                    if (!campusSubjectStudents.ContainsKey(key))
                    {
                        campusSubjectStudents[key] = new List<Student>();
                    }
                    campusSubjectStudents[key].Add(student);
                }
            }

            foreach (var entry in campusSubjectStudents)
            {
                var (campusId, subjectId) = entry.Key;
                var students = entry.Value;
                await AssignToGroupsByCampusAndSubject(campusId, subjectId, students, subjectType);
            }
        }

        private async System.Threading.Tasks.Task AssignToGroupsByCampusAndSubject(int campusId, int subjectId, List<Student> students, SubjectType subjectType)
        {
            if (!students.Any()) return;

            var distributedGroups = DistributeStudents(students.Count, subjectType);

            if (distributedGroups.Count() == 0)
                return;

            var groupDictionary = new Dictionary<Group, int>();
            foreach (var (groupSize, index) in distributedGroups.Select((value, i) => (value, i)))
            {
                var groupToAdd = new Group
                {
                    GroupName = $"Group {campusId}-{subjectId}-{index + 1}",
                    SubjectId = subjectId,
                    HasMentor = false,
                    IsCurrentPeriod = true,
                    Status = "Eligible"
                };
                await _unitOfWork.Groups.InsertAsync(groupToAdd);
                groupDictionary.Add(groupToAdd, groupSize);
            }
            await _unitOfWork.SaveAsync();

            foreach (var group in groupDictionary)
            {
                for (int i = 0; i < group.Value; i++)
                {
                    var student = students[0];
                    var groupMember = new GroupMember
                    {
                        GroupId = (await _unitOfWork.Groups.GetAsync(filter: g => g.GroupName == group.Key.GroupName)).FirstOrDefault().GroupId,
                        StudentId = student.StudentId,
                        Role = i == 0 ? "Leader" : "Member"
                    };
                    students.RemoveAt(0);
                    await _unitOfWork.GroupMembers.InsertAsync(groupMember);
                }
            }
            await _unitOfWork.SaveAsync();
        }



        public async System.Threading.Tasks.Task UpdateAndAssignStudentsToGroups(SubjectType subjectType)
        {
            var studentIdSet = await UpdateEligibleGroup(subjectType);
            await AssignStudentsToGroups(studentIdSet, subjectType);
        }
    }
}
