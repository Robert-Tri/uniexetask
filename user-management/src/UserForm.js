import React, { useState } from 'react';
import axios from 'axios';
import './UserForm.css'; 

const UserForm = () => {
  const [formData, setFormData] = useState({
    fullName: '',
    password: '',
    email: '',
    phone: '',
    campusId: '',
    status: true,
    roleId: ''
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await axios.post('https://localhost:7289/api/user', formData);
      alert('Người dùng đã được tạo thành công!');
    } catch (error) {
      console.error('Có lỗi xảy ra khi tạo người dùng!', error);
    }
  };

  return (
    <form className="user-form" onSubmit={handleSubmit}>
      <input
        type="text"
        name="fullName"
        placeholder="Họ và tên"
        value={formData.fullName}
        onChange={handleChange}
        required
        className="form-input"
      />
      <input
        type="password"
        name="password"
        placeholder="Mật khẩu"
        value={formData.password}
        onChange={handleChange}
        required
        className="form-input"
      />
      <input
        type="email"
        name="email"
        placeholder="Email"
        value={formData.email}
        onChange={handleChange}
        required
        className="form-input"
      />
      <input
        type="text"
        name="phone"
        placeholder="Số điện thoại"
        value={formData.phone}
        onChange={handleChange}
        required
        className="form-input"
      />
      
      <select
        name="campusId"
        value={formData.campusId}
        onChange={handleChange}
        required
        className="form-input"
      >
        <option value="">Chọn cơ sở</option>
        <option value="1">FPT-HN</option>
        <option value="2">FPT-HCM</option>
        <option value="3">FPT-DN</option>
      </select>
      
      <select
        name="roleId"
        value={formData.roleId}
        onChange={handleChange}
        required
        className="form-input"
      >
        <option value="">Chọn vai trò</option>
        <option value="1">Admin</option>
        <option value="2">Manager</option>
        <option value="3">Student</option>
        <option value="4">Mentor</option>
        <option value="5">Sponsor</option>
      </select>
      
      <button type="submit" className="submit-button">Tạo người dùng</button>
    </form>
  );
};

export default UserForm;
