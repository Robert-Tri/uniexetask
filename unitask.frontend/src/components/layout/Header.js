import React, { useContext, useState } from 'react';
import { Link } from 'react-router-dom';
import { User, Settings, LogOut } from 'lucide-react'; 
import useAuth from "../../hooks/useAuth";
import axios from 'axios';
import { API_BASE_URL } from '../../config';

const handleLogout = async () => {
  try {
    // Gọi API logout
    await axios.post(`${API_BASE_URL}api/auth/logout`, null, {
      withCredentials: true // Đảm bảo cookie được gửi kèm theo request
    });

    // Sau khi logout thành công, điều hướng người dùng về trang login
    window.location.href = '/login'; 
  } catch (error) {
    console.error('Error logging out:', error);
  }
};

const Header = () => {
  const [showDropdown, setShowDropdown] = useState(false);

  return (
    <header className="bg-white shadow-md fixed top-0 left-0 w-full z-50"> 
      <div className="container mx-auto px-4 py-3 flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <img src="/images/logo-dai-hoc-fpt.svg" alt="Logo" className="h-10 w-35" />
          {/* Tiêu đề của ứng dụng */}
          <h1 className="text-xl font-bold text-gray-800">UniEXETask</h1>
        </div>
        <div className="flex items-center space-x-2">
          {/* Thông tin sinh viên căn phải */}
          <div className="flex flex-col items-end text-gray-600">
            <span className="text-lg font-semibold">Tên Sinh Viên</span> {/* Tên lớn hơn */}
            <span className="text-sm">Vai Trò</span> {/* Vai trò nhỏ hơn */}
          </div>
          <div className="relative">
            <img
              src="/images/avatar-user.jpg"
              alt="Avatar"
              className="h-12 w-12 rounded-full cursor-pointer" // Tăng kích thước avatar
              onClick={() => setShowDropdown(!showDropdown)}
            />
            {showDropdown && (
              <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-10">
                <a href="#" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                  <User className="inline-block mr-2" size={16} /> Profile
                </a>
                <a href="#" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                  <Settings className="inline-block mr-2" size={16} /> Setting
                </a>
                <a href="#" onClick={handleLogout} className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                  <LogOut className="inline-block mr-2" size={16} /> Logout
                </a>
              </div>
            )}
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
