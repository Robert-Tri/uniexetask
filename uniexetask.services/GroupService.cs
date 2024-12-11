using Microsoft.Net.Http.Headers;
using System.Globalization;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class GroupService : IGroupService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly int _min_member_exe101;
        private readonly int _max_member_exe101;
        private readonly int _min_member_exe201;
        private readonly int _max_member_exe201;
        public GroupService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _min_member_exe101 = _unitOfWork.ConfigSystems.GetConfigSystemByID((int)ConfigSystemName.MIN_MEMBER_EXE101)?.Number ?? 4;
            _max_member_exe101 = _unitOfWork.ConfigSystems.GetConfigSystemByID((int)ConfigSystemName.MAX_MEMBER_EXE101)?.Number ?? 6;
            _min_member_exe201 = _unitOfWork.ConfigSystems.GetConfigSystemByID((int)ConfigSystemName.MIN_MEMBER_EXE201)?.Number ?? 8;
            _max_member_exe201 = _unitOfWork.ConfigSystems.GetConfigSystemByID((int)ConfigSystemName.MAX_MEMBER_EXE201)?.Number ?? 10;
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

        public async Task<bool> DeleteGroup(int groupId)
        {
            var group = await _unitOfWork.Groups.GetByIDAsync(groupId);
            if (group != null)
            {
                group.IsDeleted = true;
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
                var mentorInGroup = await _unitOfWork.Groups.GetMentorInGroup(group.GroupId);
                if (mentorInGroup != null && chatGroup != null)
                {
                    var isUserInChatGroup = await _unitOfWork.ChatGroups.IsUserInChatGroup(chatGroup.ChatGroupId, mentorInGroup.UserId);
                    if (isUserInChatGroup)
                    {
                        var user = await _unitOfWork.Users.GetByIDAsync(mentorInGroup.UserId);
                        if (user != null)
                        {
                            chatGroup.Users.Remove(user);
                            _unitOfWork.Users.Update(user);
                            _unitOfWork.Save();
                        }
                    }
                }

                var userToAdd = await _unitOfWork.Users.GetByIDAsync(mentor.UserId);
                if (userToAdd != null && chatGroup != null)
                {
                    var isUserInChatGroup = await _unitOfWork.ChatGroups.IsUserInChatGroup(chatGroup.ChatGroupId, userToAdd.UserId);
                    if (!isUserInChatGroup)
                    {
                        chatGroup.Users.Add(userToAdd);
                        _unitOfWork.ChatGroups.Update(chatGroup);
                        _unitOfWork.Save();
                    }
                }

                await _unitOfWork.Groups.RemoveMentorFromGroup(groupId);
                group.Mentors.Add(mentor);
                _unitOfWork.Save();

                await SendEmailNotification(group, mentor, true);
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
                }

                group.Mentors.Add(mentor);
                group.HasMentor = true;
                _unitOfWork.Groups.Update(group);
                _unitOfWork.Save();

                await SendEmailNotification(group, mentor, false);
            }
        }

        private async System.Threading.Tasks.Task SendEmailNotification(Group group, Mentor mentor, bool isChangeMentor)
        {
            var groupMembers = await _unitOfWork.GroupMembers.GetGroupMembersWithStudentAndUser(group.GroupId);
            var leader = groupMembers.FirstOrDefault(gm => gm.Role == "Leader");
            var student = await _unitOfWork.Students.GetByIDAsync(leader.StudentId);
            var userStudent = await _unitOfWork.Users.GetByIDAsync(student.UserId);
            var userMentor = await _unitOfWork.Users.GetByIDAsync(mentor.UserId);

            string emailSubject;
            string emailContent;

            if (isChangeMentor)
            {
                emailSubject = "Mentor Change Notification";
                emailContent = $@"
        <p>Dear Team <strong>{group.GroupName}</strong>,</p>
        <p>Your group's mentor has been updated.</p>
        <p><strong>New Mentor Name:</strong> {userMentor.FullName}</p>
        <p>Please reach out to your new mentor for guidance.</p>";
            }
            else
            {
                emailSubject = "Mentor Assignment Notification";
                emailContent = $@"
        <p>Dear Team <strong>{group.GroupName}</strong>,</p>
        <p>We are pleased to announce that a mentor has been assigned to your group.</p>
        <p><strong>Mentor Name:</strong> {userMentor.FullName}</p>
        <p>Your mentor will assist with project planning and provide feedback. Please schedule a meeting with your mentor.</p>";
            }

            emailContent = $@"
    <!DOCTYPE html>
    <html lang='en'>
    <head>
        <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333333;
            margin: 0;
            padding: 20px;
            background-color: #f9f9f9;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #ffffff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            text-align: center;
            background-color: #4CAF50;
            color: white;
            padding: 10px 0;
            border-radius: 8px 8px 0 0;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
        }}
        .content h2 {{
            color: #4CAF50;
            font-size: 20px;
        }}
        .content p {{
            margin: 10px 0;
        }}
        .footer {{
            text-align: center;
            font-size: 12px;
            color: #777777;
            margin-top: 20px;
        }}
        .footer a {{
            color: #4CAF50;
            text-decoration: none;
        }}
    </style>
    </head>
    <body>
        <div>{emailContent}</div>
    </body>
    </html>";

#pragma warning disable CS4014
            _emailService.SendEmailAsync(userStudent.Email, emailSubject, emailContent);
#pragma warning restore CS4014
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

        public async Task<Group?> GetGroupByUserId(int userId)
        {
            var groupMember = await _unitOfWork.GroupMembers.GetAsync(gm => gm.Student.UserId == userId);

            if (groupMember == null || !groupMember.Any())
            {
                return null; // Trả về null nếu không tìm thấy nhóm nào
            }

            var groupId = groupMember.FirstOrDefault()?.GroupId;

            if (groupId == null)
            {
                return null;
            }

            var group = await _unitOfWork.Groups.GetByIDAsync(groupId.Value);

            return group; // Trả về nhóm tìm được
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
                minGroupSize = _min_member_exe101;
                maxGroupSize = _max_member_exe101;
            }
            else if (SubjectType.EXE201 == subjectType)
            {
                minGroupSize = _min_member_exe101;
                maxGroupSize = _max_member_exe201;
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
                        var chatGroup = await _unitOfWork.ChatGroups.GetChatGroupByGroupId(group.GroupId);
                        if (chatGroup != null)
                        {
                            var chatGroupWithUsers = await _unitOfWork.ChatGroups.GetChatGroupWithUsersByChatGroupIdAsync(chatGroup.ChatGroupId);
                            if (chatGroupWithUsers != null)
                            {
                                chatGroupWithUsers.Users.Clear();
                                _unitOfWork.ChatGroups.Update(chatGroupWithUsers);
                                _unitOfWork.Save();
                                _unitOfWork.ChatGroups.Delete(chatGroupWithUsers);
                                _unitOfWork.Save();
                            }
                        }
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
                var campus = await _unitOfWork.Campus.GetByIDAsync(campusId);
                var subject = await _unitOfWork.Subjects.GetByIDAsync(subjectId);
                var groupToAdd = new Group
                {
                    GroupName = $"Group {index + 1}-{campus.CampusName}-{subject.SubjectCode}-{DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture)} {DateTime.Now.Year}",
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
                    var existingChatGroup = await _unitOfWork.ChatGroups.GetChatGroupByGroupId(group.Key.GroupId);
                    if (existingChatGroup == null)
                    {
                        var newChatGroup = new ChatGroup
                        {
                            ChatGroupName = group.Key.GroupName,
                            ChatGroupAvatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg",
                            CreatedDate = DateTime.Now,
                            CreatedBy = student.UserId,
                            OwnerId = student.UserId,
                            GroupId = group.Key.GroupId,
                            LatestActivity = DateTime.Now,
                            Type = nameof(ChatGroupType.Group)
                        };
                        await _unitOfWork.ChatGroups.InsertAsync(newChatGroup);
                        _unitOfWork.Save();
                    }
                    var chatgroup = await _unitOfWork.ChatGroups.GetChatGroupByGroupId(group.Key.GroupId);
                    if (chatgroup != null)
                    {
                        var user = await _unitOfWork.Users.GetByIDAsync(student.UserId);
                        if (user != null)
                        {
                            chatgroup.Users.Add(user);
                            _unitOfWork.ChatGroups.Update(chatgroup);
                            _unitOfWork.Save();
                        }
                    }

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

        public async Task<IEnumerable<Group>> SearchGroupsByGroupNameAsync(string query)
        {
            return await _unitOfWork.Groups.SearchGroupsByGroupNameAsync(query);
        }
        
        public async Task<IEnumerable<GroupDetailsResponseModel>> GetCurrentGroupsWithMembersAndMentors()
        {
            var groups = await _unitOfWork.Groups.GetCurrentPeriodGroupsWithMembersAndMentor();

            var formattedGroups = groups.Select(group => new GroupDetailsResponseModel
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                Status = group.Status,
                GroupMembers = group.GroupMembers.Select(gm => new GroupMemberResponseModel
                {
                    StudentId = gm.StudentId,
                    FullName = gm.Student.User.FullName,
                    StudentCode = gm.Student.StudentCode,
                    CampusId = gm.Student.User.CampusId,
                    Major = gm.Student.Major ?? "N/A",
                    Role = gm.Role
                }).ToList(),
                Mentor = group.Mentors.Any() ? new MentorResponseModel
                {
                    MentorId = group.Mentors.First().MentorId,
                    FullName = group.Mentors.First().User.FullName,
                    Specialty = group.Mentors.First().Specialty
                } : null 
            }).ToList();

            return formattedGroups;
        }
        public class GroupDetailsResponseModel
        {
            public int GroupId { get; set; }
            public string GroupName { get; set; }
            public string Status { get; set; }
            public List<GroupMemberResponseModel> GroupMembers { get; set; }
            public MentorResponseModel Mentor { get; set; }
        }
        public class GroupMemberResponseModel
        {
            public int StudentId { get; set; }
            public string FullName { get; set; }
            public string StudentCode { get; set; }
            public int CampusId { get; set; }
            public string Major { get; set; }
            public string Role { get; set; }
        }
        public class MentorResponseModel
        {
            public int MentorId { get; set; }
            public string FullName { get; set; }
            public string Specialty { get; set; }
        }
    }
}
