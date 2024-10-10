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
import ChatUI from './views/chat/chatUI';
import ProjectList from './components/project/ProjectList';
import TopicList from './components/topic/TopicList';
import GroupList from './components/group/GroupList';
import GroupDetail from './components/group/GroupDetail';
import Users from './views/user/Users';
import WorkShops from './views/workshop/WorkShops';
import Timelines from './views/timeline/Timelines';
import HomeManager from './components/home/HomeManager';
import { ToastContainer, toast } from 'react-toastify';

import 'react-toastify/dist/ReactToastify.css';

const LayoutRoute = ({ children }) => (
  <Layout>
    {children}
  </Layout>
);

function App() {

  return (
    <GoogleOAuthProvider clientId="84036477180-g8du4c9m1nvh7ducvvj0mkgm3dp9pfjp.apps.googleusercontent.com">
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
              <Route path="/WorkShops" element={<LayoutRoute><WorkShops /></LayoutRoute>} />
              <Route path="/Timelines" element={<LayoutRoute><Timelines /> </LayoutRoute>} />
              <Route path="/HomeManager" element={<LayoutRoute><HomeManager /></LayoutRoute>} />
              <Route path="/chat" element={<LayoutRoute><ChatUI /></LayoutRoute>} />

            </Routes>
            <ToastContainer />
          </Router>
    </GoogleOAuthProvider>
  );
}

export default App;
