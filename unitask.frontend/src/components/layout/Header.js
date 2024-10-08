import React, { useContext, useState } from 'react';
import { Link } from 'react-router-dom'; // Import Link từ React Router
import { User, Settings } from 'lucide-react';
import useAuth from "../../hooks/useAuth";

const Header = () => {
  const {id, username, role} = useAuth()
  const [showDropdown, setShowDropdown] = useState(false);

  return (
    <header className="bg-white shadow-md">
      <div className="container mx-auto px-4 py-3 flex items-center justify-between">
        <div className="flex items-center space-x-4">
          <Link to="/home">
            <img src="/images/logo-dai-hoc-fpt.svg" alt="Logo" className="h-10 w-35 cursor-pointer" />
          </Link>          {/* Tiêu đề của ứng dụng */}
          <h1 className="text-xl font-bold text-gray-800">UniEXETask</h1>
        </div>
        <div className="flex items-center space-x-2">
          {/* Thông tin sinh viên căn phải */}
          <div className="flex flex-col items-end text-gray-600">
            <span className="text-lg font-semibold">{username}</span> {/* Tên lớn hơn */}
            <span className="text-sm">{role}</span> {/* Vai trò nhỏ hơn */}
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
                  <User className="inline-block mr-2" size={16} /> Hồ sơ
                </a>
                <a href="#" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                  <Settings className="inline-block mr-2" size={16} /> Cài đặt
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
