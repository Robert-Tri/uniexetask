// MenuBar.js
import React, { useContext } from 'react';
import { AuthContext } from '../../contexts/AuthContext';
import StudentMenuBar from './StudentMenuBar';
import MentorMenuBar from './MentorMenuBar';
import ManagerMenuBar from './ManagerMenuBar';

const MenuBar = () => {
  const { user } = useContext(AuthContext);

  if (!user) {
    return null; // or a loading spinner
  }

  const role = user.role;

  if (role === '2') {
    return <ManagerMenuBar />;
  } else if (role === '3') {
    return <StudentMenuBar />;
  } else if (role === '4') {
    return <MentorMenuBar />;
  } else {
    return null;
  }
};

export default MenuBar;
