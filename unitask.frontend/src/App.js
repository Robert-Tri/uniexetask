import React from 'react';
import { GoogleOAuthProvider } from '@react-oauth/google';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/layout/Layout';
import LoginForm from './components/Login/LoginForm';
import Home from './components/user/Home';
import RolePermission from './views/role_permission/RolePermission';
import CreateUserForm from './components/user/CreateUserForm';
import MentorPendingProjects from './views/project/MentorPendingProjects';

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
                    <Route path="/role-permission" element={<LayoutRoute><RolePermission /></LayoutRoute>} />
                    <Route path="/projects/pending" element={<MentorPendingProjects />} />
                </Routes>
            </Router>
        </GoogleOAuthProvider>
    );
}

export default App;