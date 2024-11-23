using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<IEnumerable<Project>> GetAllProjectsByGroupId(int groupId)
        {
            var projects = await _unitOfWork.Projects.GetAsync(
                includeProperties: "Topic,Subject,Group",
                filter: rm => rm.GroupId == groupId && rm.IsDeleted == false && rm.IsCurrentPeriod == true);
            return projects;
        }



        public async Task<bool> CreateProject(Project project)
        {
            if (project != null)
            {
                await _unitOfWork.Projects.InsertAsync(project);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
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

        public async Task<Project?> GetProjectByUserId(int userId) =>
    await _unitOfWork.Students.GetAsync(filter: s => s.UserId == userId)
        .ContinueWith(async studentTask => studentTask.Result.FirstOrDefault() is var student && student != null
            ? await _unitOfWork.Projects.GetProjectByGroupId((int)await _unitOfWork.GroupMembers.GetGroupIdByStudentId(student.StudentId))
            : null).Unwrap();
    }
}
