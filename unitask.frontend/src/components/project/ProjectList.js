import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import styles from './ProjectList.module.css'; // Import CSS module

const ProjectList = () => {
  const [projects, setProjects] = useState([]);
  const [filteredProjects, setFilteredProjects] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    axios
      .get(`${API_BASE_URL}api/projects`)
      .then((response) => {
        if (response.data.success) {
          setProjects(response.data.data);
          setFilteredProjects(response.data.data);
        }
      })
      .catch((error) => {
        console.error('Error fetching projects:', error);
      });
  }, []);

  // Xử lý tìm kiếm khi người dùng nhập từ khóa
  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchTerm(value);

    // Lọc danh sách project dựa trên topicName và description
    const filtered = projects.filter(
      (project) =>
        project.topicName.toLowerCase().includes(value) ||
        project.description.toLowerCase().includes(value)
    );
    setFilteredProjects(filtered);
  };

  return (
    <div className={styles.projectList}>
      <input
        type="text"
        className={styles.searchBox}
        placeholder="Search by topic name or description..."
        value={searchTerm}
        onChange={handleSearch}
      />
      <ul>
        {filteredProjects.map((project) => (
          <li key={project.topicCode} className={styles.projectItem}>
            <h2>{project.topicName}</h2>
            <p>{project.description}</p>
            <p>Start Date: {new Date(project.startDate).toLocaleDateString()}</p>
            <button
              className={styles.detailsButton}
              onClick={() => alert(`Details of ${project.topicName}`)}
            >
              Details
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ProjectList;
