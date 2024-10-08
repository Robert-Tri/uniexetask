// MenuBar.js
import React, { useContext } from 'react';
import StudentMenuBar from './StudentMenuBar';
import MentorMenuBar from './MentorMenuBar';
import ManagerMenuBar from './ManagerMenuBar';
import useAuth from "../../hooks/useAuth";

const MenuBar = () => {
  const {id, username, role} = useAuth()


  if (role === 'Manager') {
    return <ManagerMenuBar />;
  } else if (role === 'Student') {
    return <StudentMenuBar />;
  } else if (role === 'Mentor') {
    return <MentorMenuBar />;
  } else {
    return null;
  }
};

export default MenuBar;
