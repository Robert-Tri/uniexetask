import React, { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
//Bên trả về group id thêm code sau
// import { useNavigate } from 'react-router-dom';

// const Navigation = ({ groupId }) => {
//   const navigate = useNavigate();

//   const handleProjectNavigation = () => {
//     navigate(`/projects?groupId=${groupId}`);
//   };
import {
    Table,
    TableHead,
    TableBody,
    TableRow,
    TableCell,
    Button,
    TextareaAutosize,
    Modal,
    TextField,
    Grid,
    MenuItem,
    Box,
    Typography,
    Container,
    Backdrop,
    CircularProgress
  } from '@mui/material';
import axios from 'axios';
import { API_BASE_URL } from '../../config';

const TopicRegistration = () => {
  const [groupParams] = useSearchParams();
  const groupId = groupParams.get('groupId');
  const [topics, setTopic] = useState([]);
  const [loading, setLoading] = useState(true);
  const [subjects, setSubject] = useState([]);
  const [openModal, setOpenModal] = useState(false);
  const [newTopic, setNewTopic] = useState({
    code: '',
    name: '',
    description: '',
    startDate: '',
    endDate: '',
    subject: '',
  });

  // Fetch existing topics
  useEffect(() => {
    const fetchTopic = async () => {
        try {
            setLoading(true);
            const response = await fetch(`${API_BASE_URL}api/topic/${groupId}`);
            if (!response.ok) {
              throw new Error('Network response was not ok');
            }
            const data = await response.json();
            setTopic(data.data);
            } catch (error) {
              console.error('Error fetching roles:', error);
            } finally {
              setLoading(false);
            }
        };

    const fetchSubject = async () => {
        try {
          setLoading(true);
          const response = await fetch(`${API_BASE_URL}api/group/subject/${groupId}`);
          if (!response.ok) {
            throw new Error('Network response was not ok');
          }
          const data = await response.json();
          setSubject(data.data);
          } catch (error) {
            console.error('Error fetching roles:', error);
          } finally {
            setLoading(false);
          }
      };

    fetchTopic();
    fetchSubject();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setNewTopic((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async () => {
    // API gọi để tạo mới topic
    await axios.post('/api/topics', newTopic);
    setOpenModal(false);
    setNewTopic({ code: '', name: '', description: '', startDate: '', endDate: '', subject: '' });
    // Cập nhật lại danh sách topic
    const response = await axios.get('/api/topics');
    setTopic(response.data);
  };

  return (
    <div>
      <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={loading}>
        <CircularProgress color="inherit" />
      </Backdrop>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Topic Code</TableCell>
            <TableCell>Topic Name</TableCell>
            <TableCell>Description</TableCell>
            <TableCell>Start Date</TableCell>
            <TableCell>End Date</TableCell>
            <TableCell>Subject</TableCell>
            <TableCell>Action</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {topics.length > 0 ? (
            topics.map((topic) => (
              <TableRow key={topic.id}>
                <TableCell>{topic.code}</TableCell>
                <TableCell>{topic.name}</TableCell>
                <TableCell>{topic.description}</TableCell>
                <TableCell>{topic.startDate}</TableCell>
                <TableCell>{topic.endDate}</TableCell>
                <TableCell>{topic.subject}</TableCell>
                <TableCell>
                  {/* Các nút hành động nếu cần */}
                </TableCell>
              </TableRow>
            ))
          ) : (
            <TableRow>
              <TableCell colSpan={7} align="center">
                <Button variant="contained" onClick={() => setOpenModal(true)}>
                    Register Topic
                </Button>
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>

      <Modal open={openModal} onClose={() => setOpenModal(false)}>
        <Box
          sx={{
            position: 'absolute',
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            width: '400px',
            bgcolor: 'background.paper',
            borderRadius: 2,
            boxShadow: 24,
            p: 4,
          }}
        >
          <Typography variant="h6" gutterBottom>
            Register Topic
          </Typography>
          <TextField
            name="code"
            label="Topic Code"
            value={newTopic.code}
            onChange={handleChange}
            fullWidth
            margin="normal"
          />
          <TextField
            name="name"
            label="Topic Name"
            value={newTopic.name}
            onChange={handleChange}
            fullWidth
            margin="normal"
          />
          <TextareaAutosize
            aria-label="Description"
            minRows={4}
            maxRows={4}
            placeholder="Enter description here..."
            onChange={handleChange}
            style={{
                width: '100%',
                height: '10px',
                marginTop: 8,
                marginBottom: 8,
                padding: 15,
                fontSize: '16px',
                border: `1px solid #c4c4c4`,
                borderRadius: '4px',
                resize: 'none',
                outline: '#fff',
              }}          />
          <TextField
            name="startDate"
            label="Start Date"
            type="date"
            value={newTopic.startDate}
            onChange={handleChange}
            InputLabelProps={{ shrink: true }}
            fullWidth
            margin="normal"
          />
          <TextField
            name="endDate"
            label="End Date"
            type="date"
            value={newTopic.endDate}
            onChange={handleChange}
            InputLabelProps={{ shrink: true }}
            fullWidth
            margin="normal"
          />
          <TextField
            select
            name="subject"
            label="Subject"
            value={newTopic.subject}
            onChange={handleChange}
            fullWidth
            margin="normal"
          >
            {subjects.map((subject) => (
              <MenuItem key={subject.id} value={subject.name}>
                {subject.name}
              </MenuItem>
            ))}
          </TextField>
          <Grid container spacing={2} justifyContent="flex-end" style={{ marginTop: '16px' }}>
            <Grid item>
              <Button onClick={() => setOpenModal(false)}>Cancel</Button>
            </Grid>
            <Grid item>
              <Button variant="contained" onClick={handleSubmit}>
                Send
              </Button>
            </Grid>
          </Grid>
        </Box>
      </Modal>
    </div>
  );
};

export default TopicRegistration;
