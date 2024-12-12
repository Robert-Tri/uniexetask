using AutoMapper.Execution;
using System.Collections.Generic;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class MemberScoreService : IMemberScoreService
    {
        public IUnitOfWork _unitOfWork;

        public MemberScoreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddMemberScore(List<MemberScore> memberScores)
        {
            foreach (var memberScore in memberScores) 
            {
                await _unitOfWork.MemberScores.InsertAsync(memberScore);
            }
            var result = _unitOfWork.Save();
            if(result > 0)
                return true;
            return false;
        }


        public async Task<MemberScoreResult> GetMemberScores(int projectId, int milestoneId)
        {
            var milestone = await _unitOfWork.Milestones.GetAsync(m => m.MilestoneId == milestoneId);

            if (milestone == null)
            {
                return new MemberScoreResult
                {
                    Members = new List<MemberScoreDetail>()
                };
            }
            var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
            var members = await _unitOfWork.GroupMembers.GetGroupMembersWithStudentAndUser(project.GroupId);

            if (members == null || !members.Any())
            {
                return new MemberScoreResult
                {
                    Members = new List<MemberScoreDetail>()
                };
            }

            var memberScores = await _unitOfWork.MemberScores.GetAsync(
                filter: ms => ms.ProjectId == projectId && ms.MilestoneId == milestoneId
            );

            var result = members.Select(member =>
            {
                var score = memberScores.FirstOrDefault(ms => ms.StudentId == member.Student.StudentId);
                return new MemberScoreDetail
                {
                    StudentId = member.Student.StudentId,
                    StudentName = member.Student.User.FullName,
                    StudentCode = member.Student.StudentCode,
                    Role = member.Role,
                    Score = score?.Score ?? -1,
                    Comment = score?.Comment ?? "No comment"
                };
            }).ToList();

            return new MemberScoreResult
            {
                Members = result
            };
        }

        public async Task<TotalMemberScoreResult> GetTotalMemberScore(int projectId)
        {
            var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
            if (project == null)
            {
                return new TotalMemberScoreResult
                {
                    Members = new List<MemberTotalScoreDetail>()
                };
            }

            var milestones = await _unitOfWork.Milestones.GetAsync(m => m.SubjectId == project.SubjectId && !m.IsDeleted);
            if (milestones == null || !milestones.Any())
            {
                return new TotalMemberScoreResult
                {
                    Members = new List<MemberTotalScoreDetail>()
                };
            }

            var members = await _unitOfWork.GroupMembers.GetGroupMembersWithStudentAndUser(project.GroupId);
            if (members == null || !members.Any())
            {
                return new TotalMemberScoreResult
                {
                    Members = new List<MemberTotalScoreDetail>()
                };
            }

            var allMemberScores = await _unitOfWork.MemberScores.GetAsync(ms => ms.ProjectId == projectId);

            var result = members.Select(member =>
            {
                var milestoneScores = new List<MemberMilestoneScore>();
                double totalScore = 0;

                foreach (var milestone in milestones)
                {
                    var scoreRecord = allMemberScores.FirstOrDefault(ms => ms.StudentId == member.Student.StudentId && ms.MilestoneId == milestone.MilestoneId);
                    var milestoneScore = scoreRecord?.Score ?? -1; 

                    milestoneScores.Add(new MemberMilestoneScore
                    {
                        MilestoneId = milestone.MilestoneId,
                        Score = milestoneScore, 
                        Percentage = milestone.Percentage
                    });

                    if (milestoneScore != -1)
                    {
                        totalScore += milestoneScore * milestone.Percentage / 100.0;
                    }
                }

                return new MemberTotalScoreDetail
                {
                    StudentId = member.Student.StudentId,
                    StudentName = member.Student.User.FullName,
                    StudentCode = member.Student.StudentCode,
                    Role = member.Role,
                    MilestoneScores = milestoneScores,
                    TotalScore = Math.Round(totalScore, 2)
                };
            }).ToList();

            return new TotalMemberScoreResult
            {
                Members = result
            };
        }

        public async Task<TotalMemberScoreResult> GetTotalMemberScoreV2(int projectId)
        {
            var project = await _unitOfWork.Projects.GetByIDAsync(projectId);
            if (project == null)
            {
                return new TotalMemberScoreResult
                {
                    Members = new List<MemberTotalScoreDetail>()
                };
            }
            var members = await _unitOfWork.GroupMembers.GetGroupMembersWithStudentAndUser(project.GroupId);
            if (members == null || !members.Any())
            {
                return new TotalMemberScoreResult
                {
                    Members = new List<MemberTotalScoreDetail>()
                };
            }
            // Lấy toàn bộ điểm của thành viên dựa trên projectId
            var allMemberScores = await _unitOfWork.MemberScores.GetAsync(ms => ms.ProjectId == projectId);

            // Lấy danh sách milestone liên quan
            var milestoneIds = allMemberScores.Select(ms => ms.MilestoneId).Distinct().ToList();
            var milestones = await _unitOfWork.Milestones.GetAsync(m => milestoneIds.Contains(m.MilestoneId));

            List<Milestone> milestoneList = new List<Milestone>();
            foreach (var item in milestoneIds)
            {
                var milestone = await _unitOfWork.Milestones.GetByIDAsync(item);
                milestone.MemberScores = null;
                milestoneList.Add(milestone);
            }

            // Tính điểm từng thành viên
            var memberScores = members.Select(student => {
                var memberScoresForStudent = allMemberScores.Where(ms => ms.StudentId == student.StudentId).ToList();

                // Tính điểm cho từng milestone
                var milestoneScores = milestones.Select(milestone => {
                    var scoreRecord = memberScoresForStudent.FirstOrDefault(ms => ms.MilestoneId == milestone.MilestoneId);
                    return new MemberMilestoneScore
                    {
                        MilestoneId = milestone.MilestoneId,
                        Score = scoreRecord != null ? scoreRecord.Score : -1, // -1 nếu không có điểm
                        Percentage = milestone.Percentage
                    };
                }).ToList();

                // Tổng điểm của thành viên (bỏ qua milestone không có điểm)
                double totalScore = milestoneScores
                    .Where(ms => ms.Score != -1)
                    .Sum(ms => ms.Score * (ms.Percentage / 100.0));

                return new MemberTotalScoreDetail
                {
                    StudentId = student.Student.StudentId,
                    StudentName = student.Student.User.FullName,
                    StudentCode = student.Student.StudentCode,
                    Role = student.Role,
                    MilestoneScores = milestoneScores,
                    TotalScore = Math.Round(totalScore, 2)
                };
            }).ToList();

            // Kết quả cuối cùng
            return new TotalMemberScoreResult
            {
                Members = memberScores,
                MilestoneIds = milestoneList
            };
        }


    }

    public class MemberScoreResult
    {
        public List<MemberScoreDetail> Members { get; set; }
    }

    public class MemberScoreDetail
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string Role { get; set; }
        public double Score { get; set; }
        public string Comment { get; set; }
    }
    public class TotalMemberScoreResult
    {
        public List<MemberTotalScoreDetail> Members { get; set; }
        public List<Milestone>? MilestoneIds { get; set; }
    }

    public class MemberTotalScoreDetail
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentCode { get; set; }
        public string Role { get; set; }
        public double TotalScore { get; set; }
        public List<MemberMilestoneScore> MilestoneScores { get; set; }
    }

    public class MemberMilestoneScore
    {
        public int MilestoneId { get; set; }
        public double Score { get; set; }
        public double Percentage { get; set; }
    }
}

