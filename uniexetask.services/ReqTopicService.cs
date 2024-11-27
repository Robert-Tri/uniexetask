using System.Runtime.CompilerServices;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Interfaces;

namespace uniexetask.services
{
    public class ReqTopicService : IReqTopicService
    {
        public IUnitOfWork _unitOfWork;
        public ReqTopicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<RegTopicForm>> GetAllReqTopic()
        {
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync(
                 filter: rm => rm.Status == true,
                 includeProperties: "Group,Group.Subject");
            return reqTopicList;
        }

        public async Task<IEnumerable<int>> GetGroupByReqTopic()
        {
            // Lấy danh sách yêu cầu chủ đề với trạng thái true
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync(
               
            );

            // Sử dụng LINQ để nhóm theo groupId và chỉ lấy phần tử đầu tiên của mỗi nhóm
            var distinctGroupIds = reqTopicList
                .GroupBy(rt => rt.GroupId) // Nhóm theo GroupId
                .Select(group => group.Key) // Lấy GroupId của mỗi nhóm
                .ToList();

            return distinctGroupIds;
        }

        public async Task<List<RegTopicForm>> GetReqTopicByGroupId(int groupId)
        {
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync(
                filter: rm => rm.Status == true && rm.GroupId == groupId
            );

            return reqTopicList.ToList();
        }

        public async Task<List<RegTopicForm>> GetReqTopicByUserId(int userId)
        {
            var groupMember = await _unitOfWork.GroupMembers.GetAsync(
                filter: gm => gm.Student.UserId == userId,
                includeProperties: "Group"
            );

            if (groupMember == null || !groupMember.Any())
            {
                return new List<RegTopicForm>();
            }

            var groupId = groupMember.FirstOrDefault()?.GroupId;

            if (groupId == null)
            {
                return new List<RegTopicForm>();
            }

            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync(
                filter: rt => rt.Status == true && rt.GroupId == groupId,
                includeProperties: "Group"
            );

            return reqTopicList.ToList();
        }

        public async Task<List<RegTopicForm>> GetReqTopicByMentorId(int mentorId)
        {
            // Bước 1: Lấy danh sách các Group mà Mentor quản lý
            var groups = await _unitOfWork.Groups.GetAsync(
                filter: g => g.Mentors.Any(m => m.MentorId == mentorId), // Mentor liên quan đến nhóm
                includeProperties: "RegTopicForms" // Include để lấy RegTopicForms của từng Group
            );

            if (groups == null || !groups.Any())
            {
                return new List<RegTopicForm>();
            }

            // Bước 2: Lấy tất cả các RegTopicForm từ các nhóm quản lý
            var reqTopics = groups
                .SelectMany(g => g.RegTopicForms) // Truy xuất RegTopicForms từ từng Group
                .Where(rt => rt.Status) // Chỉ lấy các topic có Status = true
                .ToList();

            return reqTopics;
        }




        public async Task<List<RegTopicForm>> GetReqTopicByDescription(string description)
        {
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync(
                filter: rm => rm.Status == true && rm.Description == description
            );

            return reqTopicList.ToList();
        }

        public async Task<bool> CreateReqTopic(RegTopicForm reqTopic)
        {
            if (reqTopic != null)
            {
                await _unitOfWork.ReqTopic.InsertAsync(reqTopic);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public async Task<RegTopicForm?> GetReqTopicById(int id)
        {
            if (id > 0)
            {
                var ReqTopic = await _unitOfWork.ReqTopic.GetByIDAsync(id);
                if (ReqTopic != null)
                {
                    return ReqTopic;
                }
            }
            return null;
        }

        public async Task<Mentor> GetMentorGroupByUserId(int userId)
        {
            var mentor = await _unitOfWork.Mentors.GetAsync(filter: rm => rm.UserId == userId);
            var mentorResult = mentor.FirstOrDefault();  // Lấy phần tử đầu tiên nếu có
            if (mentorResult == null)
            {
                // Xử lý khi không tìm thấy mentor
                return null;
            }
            return mentorResult;
        }



        public async Task<bool> UpdateReqTopic(RegTopicForm ReqTopics)
        {
            if (ReqTopics != null)
            {
                var ReqTopic = await _unitOfWork.ReqTopic.GetByIDAsync(ReqTopics.RegTopicId);
                if (ReqTopic != null)
                {
                    ReqTopic.TopicName = ReqTopics.TopicName;
                    ReqTopic.Description = ReqTopics.Description;
                    ReqTopic.Status = ReqTopics.Status;

                    _unitOfWork.ReqTopic.Update(ReqTopic);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<bool> UpdateApproveTopic(int reqTopicId )
        {
                var ReqTopic = await _unitOfWork.ReqTopic.GetByIDAsync(reqTopicId);
                if (ReqTopic != null)
                {
                    ReqTopic.Status = false;

                    _unitOfWork.ReqTopic.Update(ReqTopic);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            return false;
        }



        public async Task<string> GetMaxTopicCode()
        {
            var reqTopicList = await _unitOfWork.ReqTopic.GetAsync();
            var maxTopicCode = reqTopicList
                .Where(t => t.TopicCode.StartsWith("TP"))
                .OrderByDescending(t => t.TopicCode)
                .Select(t => t.TopicCode)
                .FirstOrDefault();

            return maxTopicCode;
        }
    }
}
