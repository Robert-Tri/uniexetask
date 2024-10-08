// AuthContext.js
import React, { createContext, useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);

  const fetchUserInfo = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}api/auth/userinfo`, { withCredentials: true });
      console.log('Fetched User Info:', response.data); 
      setUser(response.data);
    } catch (err) {
      console.error('Failed to fetch user info', err);
      setUser(null);
    }
  };

  useEffect(() => {
    fetchUserInfo();
  }, []);

  return (
    <AuthContext.Provider value={{ user, setUser, fetchUserInfo }}>
      {children}
    </AuthContext.Provider>
  );
};
