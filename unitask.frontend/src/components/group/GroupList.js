import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom'; // Sử dụng useNavigate
import { API_BASE_URL } from '../../config';
import styles from './GroupList.module.css'; // Import CSS module

const GroupList = () => {
  const [groups, setGroups] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredGroups, setFilteredGroups] = useState([]);
  const [isPopupVisible, setIsPopupVisible] = useState(false);
  const [groupName, setGroupName] = useState('');
  const [subjectId, setSubjectId] = useState('1'); // Giá trị mặc định là 1 (EXE101)
  const [studentCodes, setStudentCodes] = useState(['']); // Mảng để lưu danh sách mã sinh viên
  const navigate = useNavigate(); // Khởi tạo useNavigate

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

  // Lọc danh sách nhóm theo tên nhóm
  useEffect(() => {
    const results = groups.filter(group =>
      group.groupName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredGroups(results);
  }, [searchTerm, groups]);

  // Hàm để xử lý khi thêm một ô nhập StudentCode
  const handleAddStudentCode = () => {
    setStudentCodes([...studentCodes, '']); // Thêm một phần tử rỗng vào mảng studentCodes
  };

  // Hàm xử lý khi thay đổi giá trị mã sinh viên
  const handleStudentCodeChange = (index, value) => {
    const newStudentCodes = [...studentCodes];
    newStudentCodes[index] = value; // Cập nhật mã sinh viên tại vị trí cụ thể
    setStudentCodes(newStudentCodes);
  };

  // Hàm xử lý khi nhấn nút xoá sinh viên
  const handleRemoveStudentCode = (index) => {
    const newStudentCodes = studentCodes.filter((_, i) => i !== index); // Xoá mã sinh viên tại vị trí chỉ định
    setStudentCodes(newStudentCodes);
  };

  // Hàm xử lý khi nhấn nút Tạo Group
  const handleCreateGroup = async () => {
    try {
      const data = {
        group: {
          groupName,
          subjectId: parseInt(subjectId),
          hasMentor: false, // Đặt mặc định
          status: "Initialized" // Đặt mặc định
        },
        studentCodes: studentCodes.filter(code => code.trim() !== '') // Chỉ gửi các mã sinh viên không rỗng
      };

      const response = await axios.post(`${API_BASE_URL}api/groupMember/CreateGroupWithMember`, data);
      console.log(response.data); // In ra phản hồi từ API
      if (response.data) {
        alert('Tạo nhóm thành công!');
        setIsPopupVisible(false); // Đóng popup sau khi thành công
        setGroupName('');
        setSubjectId('1');
        setStudentCodes(['']);
        // Gọi lại danh sách nhóm sau khi tạo nhóm thành công
        fetchGroups();
      }
    } catch (error) {
      console.error('Lỗi khi tạo nhóm:', error);
      alert('Tạo nhóm thất bại!');
    }
  };

  // Xử lý sự kiện click vào nhóm để điều hướng tới trang chi tiết nhóm
  const handleGroupClick = (groupId) => {
    navigate(`/group-detail/${groupId}`); // Điều hướng tới trang chi tiết với groupId
  };

  return (
    <div>
      <button onClick={() => setIsPopupVisible(true)} className={styles.createButton}>
        Tạo group
      </button>

      {/* Hiển thị Popup khi isPopupVisible là true */}
      {isPopupVisible && (
        <div className={styles.popup}>
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
                  className={`${styles.input} ${styles.studentInput}`} // Thêm lớp studentInput
                />
                <button onClick={() => handleRemoveStudentCode(index)} className={styles.removeButton}>
                  Xoá
                </button>
              </div>
            ))}

            <button onClick={handleAddStudentCode} className={styles.addButton}>
              + Thêm sinh viên
            </button>

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

      {/* Danh sách nhóm vẫn được hiển thị bên dưới */}
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
