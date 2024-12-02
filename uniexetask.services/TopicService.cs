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
    public class TopicService : ITopicService
    {
        public IUnitOfWork _unitOfWork;
        public TopicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Topic>> GetAllTopics()

        {
            var topic = await _unitOfWork.Topics.GetAllTopics();
            return topic;
        }

        public async Task<Topic> GetTopicById(int id)
        {
            var topics = await _unitOfWork.Topics.GetByIDAsync(id);
            return topics;
        }

        public async Task<IEnumerable<Topic>> GetTopicsByMentorAsync(int mentorId)
        {
            // Lấy mentor và danh sách nhóm mà mentor quản lý
            var mentorWithGroups = await _unitOfWork.Mentors.GetMentorWithGroupAsync(mentorId);

            if (mentorWithGroups == null || !mentorWithGroups.Groups.Any())
            {
                return new List<Topic>(); // Trả về danh sách rỗng nếu mentor không có nhóm
            }

            // Lấy danh sách GroupId từ các nhóm của mentor
            var groupIds = mentorWithGroups.Groups.Select(group => group.GroupId).ToList();

            // Lấy danh sách Project dựa trên GroupId
            var projects = await _unitOfWork.Projects.GetAsync(
                filter: project => groupIds.Contains(project.GroupId)
            );

            if (projects == null || !projects.Any())
            {
                return new List<Topic>(); // Trả về danh sách rỗng nếu không có project nào
            }

            // Lấy danh sách TopicId từ các project
            var topicIds = projects.Select(project => project.TopicId).Distinct().ToList();

            // Lấy danh sách Topic dựa trên TopicId
            var topics = await _unitOfWork.Topics.GetAsync(
                filter: topic => topicIds.Contains(topic.TopicId)
            );

            return topics;
        }



        public async Task<IEnumerable<Topic>> GetTopicByTopicName(string topicName)
        {
            var topicList = await _unitOfWork.Topics.GetAsync(
                filter: t => t.TopicName == topicName
            );
            return topicList;
        }

        public async Task<int> CreateTopic(Topic topic)
        {
            if (topic != null)
            {
                // Insert topic vào cơ sở dữ liệu
                await _unitOfWork.Topics.InsertAsync(topic);

                // Lưu thay đổi
                var result = _unitOfWork.Save();

                if (result > 0)
                {
                    // Trả về TopicId sau khi tạo thành công
                    return topic.TopicId;
                }
            }

            // Nếu có lỗi hoặc không thành công, trả về 0 (hoặc giá trị khác thể hiện thất bại)
            return 0;
        }

    }
}
