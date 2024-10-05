import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom'; // Sử dụng useNavigate
import { API_BASE_URL } from '../../config';
import './GroupList.css';

const GroupList = () => {
  const [groups, setGroups] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredGroups, setFilteredGroups] = useState([]);
  const navigate = useNavigate(); // Khởi tạo useNavigate

  useEffect(() => {
    // Hàm lấy danh sách nhóm
    const fetchGroups = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}api/group/group-subject`);
        if (response.data.success) {
          setGroups(response.data.data);
        }
      } catch (error) {
        console.error('Lỗi khi tải danh sách nhóm:', error);
      }
    };

    fetchGroups();
  }, []);

  useEffect(() => {
    // Lọc danh sách nhóm theo tên nhóm hoặc môn học
    const results = groups.filter(group =>
      group.groupName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredGroups(results);
  }, [searchTerm, groups]);

  // Xử lý sự kiện click vào nhóm để điều hướng tới trang chi tiết nhóm
  const handleGroupClick = (groupId) => {
    navigate(`/group-detail/${groupId}`); // Điều hướng tới trang chi tiết với groupId
  };

  return (
    <div>
      <input
        type="text"
        placeholder="Tìm kiếm nhóm theo tên hoặc môn học..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      <ul className='group-list'>
        {filteredGroups.map(group => (
          <li key={group.groupId} onClick={() => handleGroupClick(group.groupId)}>
            <h2>{group.groupName}</h2>
            <p>Môn học: {group.subjectName}</p>
            <p>Có mentor: {group.hasMentor ? 'Có' : 'Không'}</p>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default GroupList;
