import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { API_BASE_URL } from '../../config';
import styles from './GroupList.module.css';

const GroupList = () => {
  const [groups, setGroups] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredGroups, setFilteredGroups] = useState([]);
  const [isPopupVisible, setIsPopupVisible] = useState(false);
  const [groupName, setGroupName] = useState('');
  const [subjectId, setSubjectId] = useState('1'); 
  const [studentCodes, setStudentCodes] = useState(['']);
  const navigate = useNavigate();
  const popupRef = useRef(null); // Tạo ref cho popup

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

  useEffect(() => {
    fetchGroups();
  }, []);

  useEffect(() => {
    const results = groups.filter(group =>
      group.groupName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredGroups(results);
  }, [searchTerm, groups]);

  const handleAddStudentCode = () => {
    setStudentCodes([...studentCodes, '']);
  };

  const handleStudentCodeChange = (index, value) => {
    const newStudentCodes = [...studentCodes];
    newStudentCodes[index] = value;
    setStudentCodes(newStudentCodes);
  };

  const handleRemoveStudentCode = (index) => {
    const newStudentCodes = studentCodes.filter((_, i) => i !== index);
    setStudentCodes(newStudentCodes);
  };

  const handleCreateGroup = async () => {
    try {
      const data = {
        group: {
          groupName,
          subjectId: parseInt(subjectId),
          hasMentor: false,
          status: "Initialized"
        },
        studentCodes: studentCodes.filter(code => code.trim() !== '')
      };

      const response = await axios.post(`${API_BASE_URL}api/groupMember/CreateGroupWithMember`, data);
      if (response.data) {
        alert('Tạo nhóm thành công!');
        setIsPopupVisible(false);
        setGroupName('');
        setSubjectId('1');
        setStudentCodes(['']);
        fetchGroups();
      }
    } catch (error) {
      console.error('Lỗi khi tạo nhóm:', error);
      alert('Tạo nhóm thất bại!');
    }
  };

  const handleGroupClick = (groupId) => {
    navigate(`/group-detail/${groupId}`);
  };

  // Lắng nghe sự kiện click ngoài Popup
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (popupRef.current && !popupRef.current.contains(event.target)) {
        setIsPopupVisible(false); // Ẩn Popup nếu click bên ngoài
      }
    };

    if (isPopupVisible) {
      document.addEventListener('mousedown', handleClickOutside);
    } else {
      document.removeEventListener('mousedown', handleClickOutside);
    }

    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isPopupVisible]);

  return (
    <div>
      <button onClick={() => setIsPopupVisible(true)} className={styles.createButton}>
        Tạo group
      </button>

      {isPopupVisible && (
        <div className={styles.popup} ref={popupRef}>
          <div className={styles.popupContent}>
            <h2 className={styles.popupTitle}>Tạo Group Mới</h2>
            <label>Tên nhóm:</label>
            <input
              type="text"
              value={groupName}
              onChange={(e) => setGroupName(e.target.value)}
              className={styles.input}
            />
            <label>Môn học:</label>
            <select
              value={subjectId}
              onChange={(e) => setSubjectId(e.target.value)}
              className={styles.input}
            >
              <option value="1">EXE101</option>
              <option value="2">EXE201</option>
            </select>
            <h3>Thành viên</h3>
            {studentCodes.map((code, index) => (
              <div key={index} className={styles.studentCodeContainer}>
                <input
                  type="text"
                  value={code}
                  onChange={(e) => handleStudentCodeChange(index, e.target.value)}
                  placeholder={`Mã sinh viên ${index + 1}`}
                  className={`${styles.input} ${styles.studentInput}`}
                />
                <button onClick={() => handleRemoveStudentCode(index)} className={styles.removeButton}>
                  Xoá
                </button>
              </div>
            ))}

<div className={styles.addButtonContainer}>
  <button onClick={handleAddStudentCode} className={styles.addButton}>
    + Thêm sinh viên
  </button>
</div>


            <div className={styles.popupActions}>
              <button onClick={handleCreateGroup} className={styles.createButton}>
                Tạo group
              </button>
              <button onClick={() => setIsPopupVisible(false)} className={styles.cancelButton}>
                Hủy
              </button>
            </div>
          </div>
        </div>
      )}

      <input
        type="text"
        placeholder="Tìm kiếm nhóm theo tên hoặc môn học..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        className={styles.input}
      />
      <ul className={styles.groupList}>
        {filteredGroups.map(group => (
          <li key={group.groupId} onClick={() => handleGroupClick(group.groupId)} className={styles.groupItem}>
            <h2 className={styles.groupName}>{group.groupName}</h2>
            <p className={styles.groupInfo}>Môn học: {group.subjectName}</p>
            <p className={styles.groupInfo}>Có mentor: {group.hasMentor ? 'Có' : 'Không'}</p>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default GroupList;
