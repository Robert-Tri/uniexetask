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
