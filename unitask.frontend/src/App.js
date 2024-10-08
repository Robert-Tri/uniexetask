import React from 'react';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import LoginForm from './components/Login/LoginForm';
import Home from './components/home/Home';
import RolePermission from './views/role_permission/RolePermission';
import CreateUserForm from './components/User/CreateUserForm';
import AddMemberForm from './components/User/AddMemberForm';
import MentorPendingProjects from './views/project/MentorPendingProjects';
import TopicRegistration from './views/project/TopicRegistration';
import ProjectList from './components/project/ProjectList';
import TopicList from './components/topic/TopicList';
import GroupList from './components/group/GroupList';
import GroupDetail from './components/group/GroupDetail';
import Users from './components/User/Users';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { AuthProvider } from './contexts/AuthContext';

const LayoutRoute = ({ children }) => (
  <Layout>
    {children}
  </Layout>
);

function App() {

  return (
    <GoogleOAuthProvider clientId="84036477180-g8du4c9m1nvh7ducvvj0mkgm3dp9pfjp.apps.googleusercontent.com">
        <AuthProvider>
          <Router>
            <Routes>
              <Route path="/" element={<LoginForm />} />
              <Route path="/home" element={<LayoutRoute><Home /></LayoutRoute>} />
              <Route path="/createUser" element={<LayoutRoute><CreateUserForm /></LayoutRoute>} />
              <Route path="/addMember" element={<LayoutRoute><AddMemberForm /></LayoutRoute>} />
              <Route path="/role-permission" element={<LayoutRoute><RolePermission /></LayoutRoute>} />
              <Route path="/projects/pending" element={<LayoutRoute><MentorPendingProjects /></LayoutRoute>} />
              <Route path="/projects/register" element={<LayoutRoute><TopicRegistration /></LayoutRoute>} />
              <Route path="/projects" element={<LayoutRoute><ProjectList /></LayoutRoute>} />
              <Route path="/topics" element={<LayoutRoute><TopicList /></LayoutRoute>} />
              <Route path="/groups" element={<LayoutRoute><GroupList /></LayoutRoute>} />
              <Route path="/group-detail/:groupId" element={<LayoutRoute><GroupDetail /></LayoutRoute>} />
              <Route path="/Users" element={<LayoutRoute><Users /></LayoutRoute>} />
            </Routes>
            <ToastContainer />
          </Router>
        </AuthProvider>
    </GoogleOAuthProvider>
  );
}

export default App;
