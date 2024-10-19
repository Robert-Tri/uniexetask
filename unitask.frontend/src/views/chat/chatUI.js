import React, { useState, useEffect, useCallback, useRef } from 'react';
import axios from "axios";
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { debounce } from 'lodash';  // Import lodash debounce
import {
  Menu, 
  MenuItem,
  Avatar,
  Box,
  IconButton,
  InputBase,
  Typography,
  Paper,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  Divider,
  Backdrop,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Chip,
  Autocomplete 
} from "@mui/material";
import { Phone, Videocam, Send, MoreVert } from "@mui/icons-material";
import { styled } from '@mui/system';
import { API_BASE_URL } from '../../config';
import useAuth from "../../hooks/useAuth";
import { useOnlineStatus } from '../../contexts/OnlineStatusContext';

const ScrollableBox = styled(Box)(({ theme }) => ({
    '&::-webkit-scrollbar': {
      width: '8px',
    },
    '&::-webkit-scrollbar-track': {
      background: '#2D3748',
    },
    '&::-webkit-scrollbar-thumb': {
      background: '#4A5568',
      borderRadius: '4px',
    },
    '&::-webkit-scrollbar-thumb:hover': {
      background: '#718096',
    },
  }));

  const ChatUI = () => {
    const [anchorEl, setAnchorEl] = useState(null);
    const [openDialog, setOpenDialog] = useState(false); // State cho Dialog
    const [searchEmail, setSearchEmail] = useState(""); // State cho input tìm email
    const [emailSuggestions, setEmailSuggestions] = useState([]); // Gợi ý email
    const [selectedEmails, setSelectedEmails] = useState([]); // Danh sách email đã chọn
    const {onlineUsers } = useOnlineStatus();
    const [loading, setLoading] = useState(true);
    const [loadingSuggestions, setLoadingSuggestions] = useState(false);
    const [chatData, setChatData] = useState([]);
    const [chatGroupData, setChatGroupData] = useState([]);
    const [connection, setConnection] = useState(null);
    const [newMessage, setNewMessage] = useState("");
    const [currentGroupId, setCurrentGroupId] = useState(null);
    const [currentChatGroupType, setCurrentChatGroupType] = useState("");
    const [currentReceiverId, setCurrentReceiverId] = useState(null);
    const [currentChatGroupName, setCurrentChatGroupName] = useState("");
    const { id, username } = useAuth();
    const messagesEndRef = useRef(null);
    const menuOpen = Boolean(anchorEl);

    const handleMenuOpen = (event) => {
      setAnchorEl(event.currentTarget);
    };
  
    const handleMenuClose = () => {
      setAnchorEl(null);
    };
  
    const handleAddMemberClick = () => {
      setOpenDialog(true);
      handleMenuClose();
    };
  
    const handleDialogClose = () => {
      setOpenDialog(false);
      setSearchEmail("");
      setSelectedEmails([]);
    };
  
    const handleSearchChange = (event) => {
      setSearchEmail(event.target.value);
      fetchEmailSuggestions(event.target.value); // Gọi API khi có thay đổi
    };
  
    const fetchEmailSuggestions = useCallback(
      debounce(async (query) => {
        if (!query) return; // Nếu input trống, không cần tìm kiếm
        setLoadingSuggestions(true);
        try {
          const response = await axios.get(`${API_BASE_URL}api/user/search-email`, {
            params: { query },
          });
          setEmailSuggestions(response.data.data || []);
        } catch (error) {
          console.error("Error fetching email suggestions:", error);
        } finally {
          setLoadingSuggestions(false);
        }
      }, 300), // Debounce để giảm gọi API liên tục
      []
    );
  
    const handleAddEmail = (email) => {
      if (!selectedEmails.includes(email)) {
        setSelectedEmails([...selectedEmails, email]);
      }
      setSearchEmail("");
      setEmailSuggestions([]); // Xóa gợi ý sau khi chọn email
    };
  
    const handleRemoveEmail = (email) => {
      setSelectedEmails(selectedEmails.filter((e) => e !== email));
    };
  
    const handleAddMembers = async () => {
      try {
        const response = await axios.post(`${API_BASE_URL}api/chat-groups/add-members`, {
          groupId: currentGroupId,
          emails: selectedEmails,
        }, {
          headers: {
            'Content-Type': 'application/json',
          },
          withCredentials: true, // Nếu backend yêu cầu cookie
        });
        if (response.data.success) {
          console.log(response.data.data);
          handleDialogClose();
      } else {
          console.error(response.data.errorMessage);
      }
      } catch (error) {
        console.error("Error adding members:", error);
      }
    };
  
    const handleDeleteChat = () => {
      console.log("Xóa cuộc trò chuyện...");
      handleMenuClose();
    };

    useEffect(() => {
      messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [chatGroupData]);

    useEffect(() => {
      const fetchData = async () => {
        try {
          setLoading(true);
          const response = await axios.get(`${API_BASE_URL}api/chat-groups/user`, {
            withCredentials: true
          });
          if (response.data.success) {
            setChatData(response.data.data);
          } else {
            console.error(response.data.errorMessage);
          }
        } catch (error) {
          console.error("Error fetching initial data:", error);
        } finally {
          setLoading(false);
        }
      };
  
      fetchData();
  
      // Set up SignalR connection
      const newConnection = new HubConnectionBuilder()
        .withUrl(`${API_BASE_URL}chathub`)
        .configureLogging(LogLevel.Information)
        .build();
  
      setConnection(newConnection);
    }, []);
  
    useEffect(() => {
        if (connection) {
            if (connection.state === 'Disconnected') {
                connection.start()
                    .then(() => {
                        console.log('SignalR Connected');
                    })
                    .catch(err => console.error('SignalR Connection Error: ', err));
            }
            if (connection.state === 'Connected') {
              connection.on("ReceiveMessages", (message) => {
                if (message.chatMessage.chatGroupId === currentGroupId)
                {
                  if (message.senderName === username) message.senderName = "You"
                  setChatGroupData(prevMessages => [...prevMessages, message]);
                }
              });
              connection.on("MessageRead", ({ userId, lastReadMessageId }) => {
              });
          }
        }
    }, [connection, currentGroupId]);
  
    const fetchMessages = useCallback(async (groupId) => {
      try {
        const response = await axios.get(`${API_BASE_URL}api/chat-groups/${groupId}/messages`, {
          withCredentials: true
        });
        if (response.data.success) {
          setChatGroupData(response.data.data);
        } else {
          console.error(response.data.errorMessage);
        }
      } catch (error) {
        console.error('Error fetching messages:', error);
      }
    }, []);
  
    const handleGroupClick = useCallback((groupId, groupName, receiverId, chatGroupType) => {
      if (receiverId !== 0) setCurrentReceiverId(receiverId)
      console.log("Selected Group:", groupName);
      setCurrentChatGroupName(groupName);
      setCurrentChatGroupType(chatGroupType);
      setCurrentGroupId(groupId);
      fetchMessages(groupId);
      
      if (connection) {
        connection.invoke("JoinChatGroup", groupId.toString())
            .then(() => console.log(`Joined chat group: ${groupId}`))
            .catch(err => console.error(`Error joining chat group: ${err}`));
    }
    }, [connection, fetchMessages, currentGroupId]);
  
    const sendMessage = async () => {
      if (connection && newMessage.trim() !== "" && currentGroupId) {
        try {
          await connection.invoke("SendMessage",id , currentGroupId.toString(), newMessage);
          setNewMessage("");
        } catch (err) {
          console.error(err);
        }
      }
    };
    return (
      <Box sx={{
        display: "flex",
        height: "100vh",
        backgroundColor: "#F9FAFB",
        flexDirection: { xs: "column", md: "row" },
      }}>
        <Backdrop sx={{ color: '#fff', zIndex: (theme) => theme.zIndex.drawer + 1 }} open={loading}>
          <CircularProgress color="inherit" />
        </Backdrop>
        
        {/* Sidebar */}
        <Box sx={{
          width: { xs: "100%", md: 400 },
          backgroundColor: "#1F2937",
          padding: 2,
          order: { xs: 2, md: 1 },
          height: '100vh',
          display: 'flex',
          flexDirection: 'column',
        }}>
          <Paper sx={{ marginBottom: 2, padding: "8px 16px", borderRadius: 2 }}>
            <InputBase placeholder="Search..." fullWidth />
          </Paper>
          <ScrollableBox sx={{
            flex: 1,
            overflowY: 'auto',
            paddingBottom: 2,
          }}>
            <List>
              {chatData.map((group, index) => (
                <React.Fragment key={index}>
                  <ListItem
                    button = "true"
                    onClick={() => handleGroupClick(group.chatGroup.chatGroupId, group.chatGroup.chatGroupName, group.receiverId, group.chatGroup.type)}
                    sx={{
                      '&:hover': {
                        backgroundColor: 'rgba(255, 255, 255, 0.1)',
                        borderRadius: '4px',
                        border: '1px solid white',
                      },
                    }}
                  >
                    <ListItemAvatar>
                      <Avatar alt={group.chatGroup.chatGroupName} src={group.chatGroup.chatGroupAvatar} />
                    </ListItemAvatar>
                    <ListItemText
                      primary={
                        <Typography sx={{ color: 'white', pointerEvents: 'none' }}>
                          {group.chatGroup.chatGroupName}
                        </Typography>
                      }
                      secondary={
                        <Typography sx={{ color: 'rgba(255, 255, 255, 0.7)', pointerEvents: 'none' }}>
                          {group.latestMessage}
                        </Typography>
                      }
                    />
                  </ListItem>
                  <Divider sx={{ backgroundColor: 'rgba(255,255,255,0.1)' }} />
                </React.Fragment>
              ))}
            </List>
          </ScrollableBox>
        </Box>
  
        {/* Chat Section */}
        <Box sx={{
          flex: 1,
          display: "flex",
          flexDirection: "column",
          backgroundColor: "white",
          borderRadius: 2,
          order: { xs: 1, md: 2 },
        }}>
          {/* Chat Header */}
        <Box sx={{
          padding: 2,
          backgroundColor: "#1F2937",
          display: "flex",
          alignItems: "center",
          justifyContent: "space-between",
          color: "white",
        }}>
            <Box>
              <Typography variant="h6">{currentChatGroupName}</Typography>
              <Typography variant="body2">
                Status: {onlineUsers[currentReceiverId] ? 'Online' : 'Offline'}
              </Typography>
            </Box>
            <Box>
              <IconButton sx={{ color: "white" }}>
                <Phone />
              </IconButton>
              <IconButton sx={{ color: "white" }}>
                <Videocam />
              </IconButton>

              {/* Nút mở Menu */}
              <IconButton onClick={handleMenuOpen} sx={{ color: "white" }}>
                <MoreVert />
              </IconButton>

              {/* Menu với các tùy chọn */}
              <Menu
                anchorEl={anchorEl}
                open={menuOpen}
                onClose={handleMenuClose}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'right' }}
                transformOrigin={{ vertical: 'top', horizontal: 'right' }}
              >
                {currentChatGroupType === "Group" && (
                  <MenuItem onClick={handleAddMemberClick}>Thêm thành viên</MenuItem>
                )}
                <MenuItem onClick={handleDeleteChat}>Xóa cuộc trò chuyện</MenuItem>
              </Menu>
              <Dialog open={openDialog} onClose={handleDialogClose} maxWidth="md" fullWidth>
                <DialogTitle>Thêm Thành Viên</DialogTitle>
                <DialogContent>
                  <Autocomplete
                    freeSolo
                    options={emailSuggestions}
                    getOptionLabel={(option) => option.email || ""} // Sử dụng email làm nhãn
                    onInputChange={(event, newValue) => handleSearchChange({ target: { value: newValue } })} // Gọi hàm khi có thay đổi
                    renderInput={(params) => (
                      <TextField
                        {...params}
                        label="Nhập email"
                        margin="dense"
                        autoComplete="off"
                        InputProps={{
                          ...params.InputProps,
                          endAdornment: (
                            <>
                              {loadingSuggestions ? <CircularProgress size={24} /> : null}
                              {params.InputProps.endAdornment}
                            </>
                          ),
                        }}
                      />
                    )}
                    onChange={(event, newValue) => {
                      if (newValue) {
                        handleAddEmail(newValue.email);
                      }
                    }}
                    renderOption={(props, option) => (
                      <ListItem
                        {...props}
                        key={option.email}
                        button = "true"
                        onClick={() => handleAddEmail(option.email)}
                      >
                        <ListItemAvatar>
                          <Avatar src={option.avatar} alt={option.fullName} />
                        </ListItemAvatar>
                        <ListItemText primary={option.fullName} secondary={option.email} />
                      </ListItem>
                    )}
                  />

                  {/* Danh sách các email đã chọn */}
                  <Box
                    sx={{
                      mt: 2,
                      display: 'flex',
                      flexWrap: 'wrap',
                      gap: 1,
                      maxHeight: '150px',
                      overflowY: 'auto',
                    }}
                  >
                    {selectedEmails.map((email, index) => (
                      <Chip
                        key={index}
                        label={email}
                        onDelete={() => handleRemoveEmail(email)}
                        color="primary"
                        sx={{ marginBottom: 1 }}
                      />
                    ))}
                  </Box>
                </DialogContent>
                <DialogActions>
                  <Button onClick={handleDialogClose} color="secondary">Hủy</Button>
                  <Button color="primary" onClick={handleAddMembers} disabled={!selectedEmails.length}>
                    Thêm
                  </Button>
                </DialogActions>
              </Dialog>
            </Box>
          </Box>
  
          {/* Chat Messages */}
          <Box sx={{
            flex: 1,
            padding: 2,
            overflowY: "auto",
            display: "flex",
            flexDirection: "column",
          }}>
            {chatGroupData.map((message, index) => (
              <Box
                key={index}
                sx={{
                  marginBottom: 2,
                  display: "flex",
                  justifyContent: message.senderName === "You" ? "flex-end" : "flex-start",
                }}
              >
                <Paper
                  sx={{
                    padding: 1.5,
                    backgroundColor: message.senderName === "You" ? "#1F2937" : "#E5E7EB",
                    color: message.senderName === "You" ? "white" : "black",
                    maxWidth: "80%",
                  }}
                >
                  <Typography>{message.chatMessage.messageContent}</Typography>
                </Paper>
              </Box>
            ))}
            <div ref={messagesEndRef} />
          </Box>
  
          {/* Chat Input */}
          <Box sx={{
            display: "flex",
            padding: 2,
            alignItems: "center",
            backgroundColor: "#F3F4F6",
          }}>
            <InputBase
              placeholder="Type your message here..."
              fullWidth
              value={newMessage}
              onChange={(e) => setNewMessage(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && sendMessage()}
              sx={{
                padding: "8px 16px",
                backgroundColor: "white",
                borderRadius: "9999px",
                marginRight: 1,
              }}
            />
            <IconButton onClick={sendMessage}>
              <Send />
            </IconButton>
          </Box>
        </Box>
      </Box>
    );
  };
  
  export default ChatUI;