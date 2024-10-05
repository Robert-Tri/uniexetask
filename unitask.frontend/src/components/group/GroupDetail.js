import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { API_BASE_URL } from '../../config';
import './GroupDetail.css'; // Nếu cần CSS

const GroupDetail = () => {
  const { groupId } = useParams(); // Lấy groupId từ URL
  const [members, setMembers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Hàm để lấy danh sách thành viên theo groupId
    const fetchMembers = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}api/groupMember/GetUsersByGroupId/${groupId}`);
        if (response.data) {
          setMembers(response.data);
        }
      } catch (error) {
        console.error('Lỗi khi tải danh sách thành viên:', error);
        setError('Không thể tải danh sách thành viên.');
      } finally {
        setLoading(false);
      }
    };

    fetchMembers();
  }, [groupId]);

  if (loading) return <div>Đang tải...</div>;
  if (error) return <div>{error}</div>;

  return (
    <div className="group-detail">
      <h1>Chi tiết nhóm</h1>
      <ul className="member-list">
        {members.map((member) => (
          <li key={member.userId}>
            <h2 className="member-name">{member.fullName}</h2>
            <p><strong>Email:</strong> {member.email}</p> {/* In đậm từ "Email:" */}
            {member.students && member.students.map((student) => (
              <div key={student.studentId}>
                <p><strong>Mã sinh viên:</strong> {student.studentCode}</p> {/* In đậm từ "Mã sinh viên:" */}
                <p><strong>Chuyên ngành:</strong> {student.major}</p> {/* In đậm từ "Chuyên ngành:" */}
              </div>
            ))}
          </li>
        ))}
      </ul>
    </div>
  );
};

export default GroupDetail;
