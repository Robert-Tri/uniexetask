import React from 'react';
import { Menu, Users, FolderKanban, Calendar, CheckSquare } from 'lucide-react';

const ManagerMenuBar = () => {
    return (
        <nav className="bg-gray-800 z-50 relative"> {/* Thêm z-index và relative */}
          <div className="container mx-auto px-4">
            <div className="flex items-center justify-between h-16">
              <div className="flex items-center">
                <div className="flex-shrink-0 text-white">
                  <Menu size={24} />
                </div>
                <div className="hidden md:block">
                  <div className="ml-10 flex items-baseline space-x-4">
                    <a href="/Users" className="text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                        <Users className="inline-block mr-1" size={16} /> Manage User
                    </a>
                    <a href="/manage-groups" className="text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                        <FolderKanban className="inline-block mr-1" size={16} /> Manage Group
                    </a>
                    <a href="/manage-timeline" className="text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                        <Calendar className="inline-block mr-1" size={16} /> Manage Timeline
                    </a>
                    <a href="/manage-workshops" className="text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                        <CheckSquare className="inline-block mr-1" size={16} /> Manage Workshops
                    </a>
                    <a href="/scoring-criteria" className="text-gray-300 hover:bg-gray-700 hover:text-white px-3 py-2 rounded-md text-sm font-medium">
                        <CheckSquare className="inline-block mr-1" size={16} /> Scoring Criteria
                    </a>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </nav>
      );
};

export default ManagerMenuBar;
