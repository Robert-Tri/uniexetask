import axios from 'axios';

const API_URL = 'https://localhost:7289/api/group';

const groupService = {
  addMentorToGroupAutomatically: async (groupId, mentorId) => {
    try {
      const response = await axios.post(`${API_URL}/addmentortogroupautomatically`, {
        groupId: groupId,
        mentorId: mentorId
      });
      return response.data;
    } catch (error) {
      console.error('Error adding mentor to group automatically:', error);
      throw error;
    }
  }
};

export default groupService;