import React, { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import styles from './findGroup.module.css';

const FindGroup = () => {
  const [groups, setGroups] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredGroups, setFilteredGroups] = useState([]);
  const [isPopupVisible, setIsPopupVisible] = useState(false);
  const [description, setDescription] = useState('');
  const [rowsPerPage, setRowsPerPage] = useState(5); // Default rows per page
  const [currentPage, setCurrentPage] = useState(1); // Current page
  const [sortField, setSortField] = useState('groupName'); // Default sort field
  const [sortOrder, setSortOrder] = useState('asc'); // Default sort order
  const popupRef = useRef(null); // Ref for popup handling

  // Fetch group list
  const fetchGroups = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}api/reqMembers`);
      if (response.data.success) {
        setGroups(response.data.data);
      }
    } catch (error) {
      console.error('Error fetching group list:', error);
    }
  };

  useEffect(() => {
    fetchGroups();
  }, []);

  useEffect(() => {
    const results = groups.filter(group =>
      group.groupName.toLowerCase().includes(searchTerm.toLowerCase())
    );

    // Sort results based on sortField and sortOrder
    results.sort((a, b) => {
      if (sortField === 'groupName') {
        return sortOrder === 'asc' 
          ? a.groupName.localeCompare(b.groupName) 
          : b.groupName.localeCompare(a.groupName);
      } else if (sortField === 'description') {
        return sortOrder === 'asc' 
          ? a.description.localeCompare(b.description) 
          : b.description.localeCompare(a.description);
      } else if (sortField === 'members') {
        return sortOrder === 'asc' 
          ? a.memberCount - b.memberCount 
          : b.memberCount - a.memberCount;
      }
      return 0;
    });

    setFilteredGroups(results);
  }, [searchTerm, groups, sortField, sortOrder]);

  // Handle Create Group with Description
  const handleCreateGroup = async () => {
    try {
      const data = { description };
      const response = await axios.post(`${API_BASE_URL}api/groupMember/CreateGroup`, data);
      if (response.data.success) {
        alert('Group created successfully!');
        setIsPopupVisible(false);
        setDescription('');
        fetchGroups();
      }
    } catch (error) {
      console.error('Error creating group:', error);
      alert('Failed to create group!');
    }
  };

  // Popup handling outside click
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (popupRef.current && !popupRef.current.contains(event.target)) {
        setIsPopupVisible(false);
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

  // Pagination logic
  const indexOfLastGroup = currentPage * rowsPerPage;
  const indexOfFirstGroup = indexOfLastGroup - rowsPerPage;
  const currentGroups = filteredGroups.slice(indexOfFirstGroup, indexOfLastGroup);

  const totalPages = Math.ceil(filteredGroups.length / rowsPerPage);

  return (
    <div className={styles.container}>
      <button onClick={() => setIsPopupVisible(true)} className={styles.createButton}>
        Create Group
      </button>

      {isPopupVisible && (
        <div className={styles.popup} ref={popupRef}>
          <div className={styles.popupContent}>
            <h2>Create New Group</h2>
            <label>Description:</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              className={styles.textarea}
              placeholder="Enter group description"
            />
            <div className={styles.popupActions}>
              <button onClick={handleCreateGroup} className={styles.createButton}>
                Create Group
              </button>
              <button onClick={() => setIsPopupVisible(false)} className={styles.cancelButton}>
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

      <input
        type="text"
        placeholder="Search groups by name..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        className={styles.searchInput}
      />

      {/* Table to display groups */}
      <table className={styles.groupTable}>
        <thead>
          <tr>
            <th onClick={() => {
              setSortField('groupName');
              setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
            }}>
              Group Name {sortField === 'groupName' && (sortOrder === 'asc' ? '▲' : '▼')}
            </th>
            <th onClick={() => {
              setSortField('description');
              setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
            }}>
              Description {sortField === 'description' && (sortOrder === 'asc' ? '▲' : '▼')}
            </th>
            <th onClick={() => {
              setSortField('members');
              setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
            }}>
              Members {sortField === 'members' && (sortOrder === 'asc' ? '▲' : '▼')}
            </th>
            <th>Action</th>
          </tr>
        </thead>
        <tbody>
          {currentGroups.map((group) => (
            <tr key={group.regMemberId}>
              <td>{group.groupName}</td>
              <td>{group.description}</td>
              <td>{group.memberCount}</td>
              <td>
                <button onClick={() => alert(`Contacting group ${group.groupName}`)} className={styles.contactButton}>
                  Contact
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      
      {/* Rows per Page Selector moved below the table */}
<div className={styles.rowsPerPage}>
  <label htmlFor="rowsPerPage">Rows per page:</label>
  <select
    id="rowsPerPage"
    value={rowsPerPage}
    onChange={(e) => setRowsPerPage(Number(e.target.value))}
  >
    <option value={5}>5</option>
    <option value={10}>10</option>
    <option value={15}>15</option>
  </select>
</div>


      {/* Pagination Controls */}
<div className={styles.pagination}>
  <button
    onClick={() => setCurrentPage(currentPage - 1)}
    disabled={currentPage === 1}
  >
    ◀
  </button>
  <span>Page {currentPage} of {totalPages}</span>
  <button
    onClick={() => setCurrentPage(currentPage + 1)}
    disabled={currentPage === totalPages}
  >
    ▶
  </button>
</div>

    </div>
  );
};

export default FindGroup;
