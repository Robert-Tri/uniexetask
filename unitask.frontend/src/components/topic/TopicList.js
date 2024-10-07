import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import styles from './TopicList.module.css'; // Import CSS module

const TopicsList = () => {
  const [topics, setTopics] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filteredTopics, setFilteredTopics] = useState([]);

  useEffect(() => {
    // Fetch data from API
    const fetchTopics = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}api/topic`);
        setTopics(response.data.data); // Lấy data từ API
        setFilteredTopics(response.data.data); // Ban đầu hiển thị toàn bộ danh sách
      } catch (error) {
        console.error('Error fetching topics:', error);
      }
    };

    fetchTopics();
  }, []);

  useEffect(() => {
    // Filter topics based on search term
    const results = topics.filter((topic) =>
      topic.topicCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
      topic.topicName.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredTopics(results);
  }, [searchTerm, topics]);

  const handleSearch = (event) => {
    setSearchTerm(event.target.value); // Update search term
  };

  return (
    <div>
      <input
        type="text"
        className={styles.searchInput} // Sử dụng CSS module class
        placeholder="Search by topic code or name..."
        value={searchTerm}
        onChange={handleSearch}
      />
      <table className={styles.table}>
        <thead>
          <tr>
            <th className={styles.th}>Topic Code</th>
            <th className={styles.th}>Topic Name</th>
            <th className={styles.th}>Description</th>
          </tr>
        </thead>
        <tbody>
          {filteredTopics.map((topic, index) => (
            <tr
              key={topic.topicId}
              className={`${styles.tbodyRow} ${index % 2 === 0 ? styles.trEven : ''} ${styles.trHover}`}
            >
              <td className={styles.td}>{topic.topicCode}</td>
              <td className={styles.td}>{topic.topicName}</td>
              <td className={styles.td}>{topic.description}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default TopicsList;
