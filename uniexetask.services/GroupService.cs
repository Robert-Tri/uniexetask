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

                    if (memberNumbers.Count() >= 4)
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
                        await _unitOfWork.GroupInvites.DeleteGroupInvites(group.GroupId);
                        await _unitOfWork.GroupMembers.DeleteGroupMembers(group.GroupId);
                        await _unitOfWork.ReqMembers.DeleteReqMemberForm(group.GroupId);
                        _unitOfWork.Groups.Delete(group);
                    }
                }
                await _unitOfWork.SaveAsync();
            }
            return studentIdSet;
        }

        private async System.Threading.Tasks.Task AssignStudentsToGroups(HashSet<int> studentIdSet)
        {
            var studentsWithoutGroup = (await _unitOfWork.Students.GetAsync(filter: s => !studentIdSet.Contains(s.StudentId) && s.IsCurrentPeriod)).ToList();
            var userIds = studentsWithoutGroup.Select(s => s.UserId).ToHashSet();
            var users = (await _unitOfWork.Users.GetAsync(filter: u => userIds.Contains(u.UserId))).ToList();

            var userCampusMap = users.ToDictionary(u => u.UserId, u => u.CampusId);

            var campusStudents = new Dictionary<int, List<Student>>()
    {
        { 1, new List<Student>() }, 
        { 2, new List<Student>() }, 
        { 3, new List<Student>() }  
    };

            foreach (var student in studentsWithoutGroup)
            {
                if (userCampusMap.TryGetValue(student.UserId, out var campusId) && campusStudents.ContainsKey(campusId))
                {
                    campusStudents[campusId].Add(student);
                }
            }

            foreach (var campus in campusStudents)
            {
                await AssignToGroupsByCampus(campus.Key, campus.Value);
            }
        }

        private async System.Threading.Tasks.Task AssignToGroupsByCampus(int campusId, List<Student> students)
        {
            if (!students.Any()) return;

            var distributedGroups = DistributeStudents(students.Count);

            var groupDictionary = new Dictionary<Group, int>();
            foreach (var (groupSize, index) in distributedGroups.Select((value, i) => (value, i)))
            {
                var groupToAdd = new Group
                {
                    GroupName = $"Group {campusId}-{index + 1}",
                    SubjectId = 1,
                    HasMentor = false,
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


        public async System.Threading.Tasks.Task UpdateAndAssignStudentsToGroups()
        {
            var studentIdSet = await UpdateEligibleGroup();
            await AssignStudentsToGroups(studentIdSet);
        }
    }
}
