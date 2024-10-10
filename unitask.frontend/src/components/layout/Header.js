import React, { useContext, useState } from 'react';
import { Link } from 'react-router-dom';
import { User, Settings, LogOut } from 'lucide-react'; 
import useAuth from "../../hooks/useAuth";
import axios from 'axios';
import { API_BASE_URL } from '../../config';

const handleLogout = async () => {
  try {
    await axios.post(`${API_BASE_URL}api/auth/logout`, null, {
      withCredentials: true // Đảm bảo cookie được gửi kèm theo request
    });
    window.location.href = '/'; 
  } catch (error) {
    console.error('Error logging out:', error);
  }
};

const Header = () => {
  const [showDropdown, setShowDropdown] = useState(false);
  const { id, username, role } = useAuth();
  
  // Đặt màu nền và màu chữ dựa trên role
  const backgroundColor = role.toLowerCase() === 'manager' ? '#281942' : '#FFFFFF'; // Màu nền
  const textColor = role.toLowerCase() === 'manager' ? '#FFFFFF' : '#000000'; // Màu chữ, mặc định là đen, Manager là trắng

  return (
    <header className="shadow-md fixed top-0 left-0 w-full" style={{ backgroundColor }}> 
      <div className="container mx-auto px-4 py-3 flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <Link to="/home">
            <img src="/images/logo-dai-hoc-fpt.svg" alt="Logo" className="h-10 w-35" />
          </Link>
          <h1 className="text-xl font-bold" style={{ color: textColor }}>UniEXETask</h1>
        </div>
        <div className="flex items-center space-x-2">
          <div className="flex flex-col items-end" style={{ color: textColor }}>
            <span className="text-lg font-semibold">{username}</span>
            <span className="text-sm">{role}</span>
          </div>
          <div className="relative">
            <img
              src="/images/avatar-user.jpg"
              alt="Avatar"
              className="h-12 w-12 rounded-full cursor-pointer"
              onClick={() => setShowDropdown(!showDropdown)}
            />
            {showDropdown && (
              <div className="absolute right-0 mt-3 w-48 bg-white rounded-md shadow-lg py-1 z-30">
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
