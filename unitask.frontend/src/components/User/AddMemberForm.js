import React, { useState } from 'react';
import axios from 'axios';
import './AddMemberForm.css';

const AddMemberForm = () => {
  const [formData, setFormData] = useState({
    groupId: '',
    studentId: ''
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post('https://localhost:7289/api/groupMember', {
        ...formData, 
        role: '' // Không cần nhập Role, API sẽ tự gán giá trị "Member" nếu để trống
      });
      alert('Thành viên đã được thêm thành công!');
    } catch (error) {
      console.error('Có lỗi xảy ra khi thêm thành viên!', error);
    }
  };

  return (
    <div className="add-member-page">
      <form className="add-member-form" onSubmit={handleSubmit}>
        <h1>Thêm Thành Viên Vào Nhóm</h1>
        
        <input
          type="number"
          name="groupId"
          placeholder="Mã nhóm"
          value={formData.groupId}
          onChange={handleChange}
          required
          className="form-input"
        />
        <input
          type="number"
          name="studentId"
          placeholder="Mã sinh viên"
          value={formData.studentId}
          onChange={handleChange}
          required
          className="form-input"
        />

        <button type="submit" className="submit-button">Thêm thành viên</button>
      </form>
    </div>
  );
};

export default AddMemberForm;
