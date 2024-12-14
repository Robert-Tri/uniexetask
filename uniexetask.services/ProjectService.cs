using Microsoft.Identity.Client;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ProjectService : IProjectService
    {
        public IUnitOfWork _unitOfWork;
        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Project>> GetAllProjects()

        {
            var project = await _unitOfWork.Projects.GetAsync(includeProperties: "Topic,Subject");
            return project;
        }

        public async Task<IEnumerable<Project>> GetAllDocuments()
        {
            var projects = await _unitOfWork.Projects.GetAsync();
            return projects;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsByGroupId(int groupId)
        {
            var projects = await _unitOfWork.Projects.GetAsync(
                includeProperties: "Topic,Subject,Group",
                filter: rm => rm.GroupId == groupId && rm.IsDeleted == false && rm.IsCurrentPeriod == true);
            return projects;
        }

        public async Task<Project> GetProjectById(int projectId)
        {
            if (projectId > 0)
            {
                var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
                if (project != null)
                {
                    return project;
                }
            }
            return null;
        }
        public async Task<Project> GetProjectWithAllDataById(int projectId)
        {
            if (projectId > 0)
            {
                var project = await _unitOfWork.Projects.GetProjectWithAllDataById(projectId);
                if (project != null)
                {
                    return project;
                }
            }
            return null;
        }

        public async Task<Project> CreateProject(Project project)
        {
            if (project != null)
            {
                await _unitOfWork.Projects.InsertAsync(project);

                var result = await _unitOfWork.SaveAsync();  

                if (result > 0)
                    return project;  
                else
                    return null;  
            }
            return null; 
        }


        public async Task<Project?> GetProjectPendingByGroupAsync(Group group)
        {
            var project = await _unitOfWork.Projects.GetProjectPendingByGroupId(group.GroupId);
            return project;
        }

        public async Task<bool> UpdateProjectStatus(int projectId, string action)
        {
            var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
            if (project == null) return false;

            if (action.Equals("accept")) 
            {
                project.Status = "Accepted";
                _unitOfWork.Save();
                return true;
            }
            else if (action.Equals("reject"))
            {
                project.Status = "Rejected";
                _unitOfWork.Save();
                return true;
            }
            else return false;
        }
        public async Task<Project?> GetProjectByStudentId(int studentId)
        {
            var groupId = await _unitOfWork.GroupMembers.GetGroupIdByStudentId(studentId);
            return await _unitOfWork.Projects.GetProjectByGroupId((int)groupId);
        }

        public async Task<Project?> GetProjectByUserId(int userId)
        {
            var student = await _unitOfWork.Students.GetAsync(filter: s => s.UserId == userId);

            if (student == null || !student.Any())
            {
                return null;
            }

            var studentEntity = student.FirstOrDefault();
            if (studentEntity == null)
            {
                return null;
            }

            var groupId = await _unitOfWork.GroupMembers.GetGroupIdByStudentId(studentEntity.StudentId);
            if (groupId == -1)
            {
                return null;
            }

            var project = await _unitOfWork.Projects.GetProjectByGroupId((int)groupId);

            return project;
        }

        public async Task<Project?> GetProjectWithTopicByGroupId(int groupId)
        {
            return await _unitOfWork.Projects.GetProjectWithTopicByGroupId(groupId);
        }

        public async System.Threading.Tasks.Task UpdateEndDurationEXE101()
        {
            var groups = await _unitOfWork.Groups.GetAsync(filter: g => g.IsCurrentPeriod == true && g.Status == nameof(GroupStatus.Approved) && g.IsDeleted == false && g.SubjectId == (int)SubjectType.EXE101);
            foreach (var group in groups) 
            {
                
                var project = (await _unitOfWork.Projects.GetAsync(filter: p => p.GroupId == group.GroupId)).FirstOrDefault();
                if(project != null)
                {
                    project.Status = nameof(ProjectStatus.Completed);
                    project.IsCurrentPeriod = false;
                    _unitOfWork.Projects.Update(project);
                }
                var groupMembers = await _unitOfWork.GroupMembers.GetAsync(filter: gm => gm.GroupId == group.GroupId);
                foreach(var groupMember in groupMembers)
                {
                    var student = await _unitOfWork.Students.GetByIDAsync(groupMember.StudentId);
                    if(student != null)
                    {
                        student.IsCurrentPeriod = false;
                        _unitOfWork.Students.Update(student);
                        var user = await _unitOfWork.Users.GetByIDAsync(student.UserId);
                        if(user != null)
                        {
                            _unitOfWork.Users.Update(user);
                            user.IsDeleted = true;
                        }
                    }
                    _unitOfWork.GroupMembers.Delete(groupMember);
                }
                group.IsCurrentPeriod = false;
                _unitOfWork.Save();
            }
        }

        public async System.Threading.Tasks.Task UpdateEndDurationEXE201()
        {
            var groups = await _unitOfWork.Groups.GetAsync(filter: g => g.IsCurrentPeriod == true && g.Status == nameof(GroupStatus.Approved) && g.IsDeleted == false && g.SubjectId == (int)SubjectType.EXE201);
            foreach (var group in groups)
            {
                var project = (await _unitOfWork.Projects.GetAsync(filter: p => p.GroupId == group.GroupId)).FirstOrDefault();
                if (project != null)
                {
                    project.Status = nameof(ProjectStatus.Completed);
                    project.IsCurrentPeriod = false;
                    _unitOfWork.Projects.Update(project);
                }
                var groupMembers = await _unitOfWork.GroupMembers.GetAsync(filter: gm => gm.GroupId == group.GroupId);
                foreach (var groupMember in groupMembers)
                {
                    var student = await _unitOfWork.Students.GetByIDAsync(groupMember.StudentId);
                    if(student != null)
                    {
                        student.IsCurrentPeriod = false;
                        _unitOfWork.Students.Update(student);
                        var user = await _unitOfWork.Users.GetByIDAsync(student.UserId);
                        if (user != null)
                        {
                            user.IsDeleted = true;
                            _unitOfWork.Users.Update(user);
                        }
                    }
                    _unitOfWork.GroupMembers.Delete(groupMember);
                }
                group.IsCurrentPeriod = false;
                _unitOfWork.Save();
            }
        }

        /*public async Task<bool> ContinueProject(int userId)
        {
            var project = await GetProjectByUserId(userId);
            if(project != null)
            {
                project.SubjectId = (int)SubjectType.EXE201;
                var group = await _unitOfWork.Groups.GetByIDAsync(project.GroupId);
                if(group != null)
                {
                    group.Status = nameof(GroupStatus.Initialized);
                    group.SubjectId = (int)SubjectType.EXE201;
                    group.HasMentor = false;
                    await _unitOfWork.Groups.RemoveMentorFromGroup(group.GroupId);
                    var result = _unitOfWork.Save();
                    if (result > 0)
                        return true;
                    return false;
                }
            }
            return false;
        }*/
    }
}
