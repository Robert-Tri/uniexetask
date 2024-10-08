import React from 'react';
import { Menu, Users, FolderKanban, Calendar, CheckSquare } from 'lucide-react';
import { Link } from 'react-router-dom';

const ManagerMenuBar = () => {
    return (
        <nav className="bg-gray-800 w-64 h-screen fixed" style={{ paddingTop: '70px', paddingLeft: '5px'}}>
        <div className="flex items-center h-16 text-white">
            <Menu size={24} />
            <span className="ml-2 font-bold">Menu</span>
          </div>
          <div className="mt-4 overflow-y-auto"> {/* Thêm overflow-y-auto để cuộn nếu cần */}
            <div className="flex flex-col space-y-2">
              <Link to="/Users" className="text-gray-300 hover:bg-gray-700 hover:text-white px-4 py-2 rounded-md text-sm font-medium flex items-center">
                  <Users className="inline-block mr-1" size={16} /> Manage User
              </Link>
              <Link to="/manage-groups" className="text-gray-300 hover:bg-gray-700 hover:text-white px-4 py-2 rounded-md text-sm font-medium flex items-center">
                  <FolderKanban className="inline-block mr-1" size={16} /> Manage Group
              </Link>
              <Link to="/manage-timeline" className="text-gray-300 hover:bg-gray-700 hover:text-white px-4 py-2 rounded-md text-sm font-medium flex items-center">
                  <Calendar className="inline-block mr-1" size={16} /> Manage Timeline
              </Link>
              <Link to="/manage-workshops" className="text-gray-300 hover:bg-gray-700 hover:text-white px-4 py-2 rounded-md text-sm font-medium flex items-center">
                  <CheckSquare className="inline-block mr-1" size={16} /> Manage Workshops
              </Link>
              <Link to="/scoring-criteria" className="text-gray-300 hover:bg-gray-700 hover:text-white px-4 py-2 rounded-md text-sm font-medium flex items-center">
                  <CheckSquare className="inline-block mr-1" size={16} /> Scoring Criteria
              </Link>
            </div>
          </div>
        </nav>
    );
};

export default ManagerMenuBar;