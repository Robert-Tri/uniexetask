import React, { useState, useEffect } from 'react';
import { Stack, TextField, Select, MenuItem, Table, TableHead, TableRow, TableCell, TableBody, Button, FormControl, InputLabel, CircularProgress, Backdrop } from '@mui/material';
import axios from 'axios';
import { API_BASE_URL } from '../../config';

const MentorPendingProjects = () => {
  const [projects, setProjects] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [filter, setFilter] = useState('all');
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchProjects = async () => {
      setLoading(true);
      try {
        const response = await axios.get(`${API_BASE_URL}api/projects/pending`, {
            withCredentials: true
        });
        setProjects(response.data.data);
      } catch (error) {
        console.error('Error fetching projects:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchProjects();
  }, []);

  const handleSearchChange = (event) => {
    setSearchQuery(event.target.value);
  };

  const handleFilterChange = (event) => {
    setFilter(event.target.value);
  };

  const handleAction = async (projectId, action) => {
    setLoading(true);
    try {
      const response = await axios.post(
        `${API_BASE_URL}api/projects/${projectId}/update-status`,
        { action },
        { withCredentials: true }
      );
  
      if (response.status === 200) {
        setProjects((prevProjects) => prevProjects.filter(p => p.id !== projectId));
        console.log(`Project ${action}ed successfully:`, response.data);
      }
    } catch (error) {
      console.error(`Error during ${action}ing project:`, error);
    } finally {
      setLoading(false);
    }
  };

  const filteredProjects = projects.filter((project) => {
    return (
      (filter === 'all' || project.status === filter) &&
      (project.groupName.toLowerCase().includes(searchQuery.toLowerCase()) ||
        project.topicName.toLowerCase().includes(searchQuery.toLowerCase()))
    );
  });

  return (
    <div>
    <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={loading}>
        <CircularProgress color="inherit" />
    </Backdrop>
    <Stack direction={{ xs: "column", sm: "row" }} spacing={2} alignItems="center">
      <TextField
        label="Search"
        variant="outlined"
        fullWidth
        value={searchQuery}
        onChange={handleSearchChange}
      />
      <FormControl variant="outlined" fullWidth>
        <InputLabel>Filter</InputLabel>
        <Select
          label="Filter"
          value={filter}
          onChange={handleFilterChange}
        >
          <MenuItem value="all">All</MenuItem>
          <MenuItem value="pending">Chờ Phê Duyệt</MenuItem>
          <MenuItem value="approved">Đã Chấp Nhận</MenuItem>
          <MenuItem value="rejected">Đã Từ Chối</MenuItem>
        </Select>
      </FormControl>
    </Stack>

      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Group Name</TableCell>
            <TableCell>Topic Name</TableCell>
            <TableCell>Description</TableCell>
            <TableCell align="center">Actions</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {filteredProjects.map((project) => (
            <TableRow key={project.id}>
              <TableCell>{project.groupName}</TableCell>
              <TableCell>{project.topic}</TableCell>
              <TableCell>{project.description}</TableCell>
              <TableCell align="center">
                <Button
                  variant="contained"
                  color="primary"
                  onClick={() => handleAction(project.id, 'accept')}
                >
                  Accept
                </Button>
                <Button
                  variant="contained"
                  color="secondary"
                  onClick={() => handleAction(project.id, 'reject')}
                  style={{ marginLeft: '10px' }}
                >
                  Reject
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
};

export default MentorPendingProjects;
