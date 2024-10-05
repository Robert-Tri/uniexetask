import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import './GroupList.css';

const GroupList = () => {
  const [groups, setGroups] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredGroups, setFilteredGroups] = useState([]);

  useEffect(() => {
    // Hàm để gọi API lấy danh sách nhóm
    const fetchGroups = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}api/group`);
        if (response.data.success) {
          setGroups(response.data.data);
        }
      } catch (error) {
        console.error('Error fetching groups:', error);
      }
    };

    fetchGroups();
  }, []);

  useEffect(() => {
    // Lọc danh sách nhóm theo tên nhóm hoặc môn học khi searchTerm thay đổi
    const results = groups.filter(group =>
      group.groupName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredGroups(results);
  }, [searchTerm, groups]);

  return (
    <div>
      <input
        type="text"
        placeholder="Search by group name or subject..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      <ul className='group-list'>
        {filteredGroups.map(group => (
          <li key={group.groupId}>
            <h2>{group.groupName}</h2>
            <p>Subject: {group.subjectName}</p>
            <p>Mentor: {group.hasMentor ? 'Yes' : 'No'}</p>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default GroupList;
