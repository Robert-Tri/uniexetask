import React, { createContext, useState, useContext, useEffect } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { API_BASE_URL } from '../config';
import useAuth from "../hooks/useAuth";

const OnlineStatusContext = createContext();

export const useOnlineStatus = () => useContext(OnlineStatusContext);

export const OnlineStatusProvider = ({ children }) => {
  const { id } = useAuth();
  const [onlineUsers, setOnlineUsers] = useState({});
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}chathub`)
      .configureLogging(LogLevel.Information)
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log('SignalR Connected');
          connection.invoke("SetUserOnline", id);
        })
        .catch(err => console.error('SignalR Connection Error: ', err));

      // Listen for the current online users when first connecting
      connection.on("ReceiveOnlineUsers", (users) => {
        const onlineStatus = {};
        users.forEach(userId => {
          onlineStatus[userId] = true; // All users in the list are online
        });
        setOnlineUsers(onlineStatus); // Set the initial online users list
      });

      connection.on("UserOnline", (userId) => {
        setOnlineUsers(prev => ({ ...prev, [userId]: true }));
      });

      connection.on("UserOffline", (userId) => {
        setOnlineUsers(prev => ({ ...prev, [userId]: false }));
      });

      return () => {
        connection.invoke("SetUserOffline", id);
        connection.stop();
      };
    }
  }, [connection, id]);

  return (
    <OnlineStatusContext.Provider value={{ onlineUsers, connection }}>
      {children}
    </OnlineStatusContext.Provider>
  );
};
