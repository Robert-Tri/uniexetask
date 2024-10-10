import React, { useState, useEffect, useCallback, useRef } from 'react';
import axios from "axios";
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import {
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
  CircularProgress
} from "@mui/material";
import { Phone, Videocam, Send } from "@mui/icons-material";
import { styled } from '@mui/system';
import { API_BASE_URL } from '../../config';
import useAuth from "../../hooks/useAuth";

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
    const [loading, setLoading] = useState(true);
    const [chatData, setChatData] = useState([]);
    const [chatGroupData, setChatGroupData] = useState([]);
    const [connection, setConnection] = useState(null);
    const [newMessage, setNewMessage] = useState("");
    const [currentGroupId, setCurrentGroupId] = useState(null);
    const { id, username } = useAuth();
    const messagesEndRef = useRef(null);

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
  
    const handleGroupClick = useCallback((groupId) => {
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
                    onClick={() => handleGroupClick(group.chatGroup.chatGroupId)}
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
              <Typography variant="h6">{chatData.user}</Typography>
              <Typography variant="body2">{chatData.status}</Typography>
            </Box>
            <Box>
              <IconButton sx={{ color: "white" }}>
                <Phone />
              </IconButton>
              <IconButton sx={{ color: "white" }}>
                <Videocam />
              </IconButton>
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