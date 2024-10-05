import React from 'react';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import LoginForm from './components/Login/LoginForm';
import Home from './components/User/Home';
import RolePermission from './views/role_permission/RolePermission';
import CreateUserForm from './components/User/CreateUserForm';
import AddMemberForm from './components/User/AddMemberForm';
import MentorPendingProjects from './views/project/MentorPendingProjects';
import ProjectList from './components/project/ProjectList';

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
                    <Route path="/projects/pending" element={<MentorPendingProjects />} />
                    <Route path="/projects" element={<LayoutRoute><ProjectList /></LayoutRoute>} />

                </Routes>
            </Router>
        </GoogleOAuthProvider>
    );
}

export default App;