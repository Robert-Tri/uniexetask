import { GoogleOAuthProvider } from '@react-oauth/google';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginForm from './components/Login/LoginForm';
import Home from './components/User/Home';
import CreateUserForm from './components/User/CreateUserForm';

function App() {
    return (
        <GoogleOAuthProvider clientId="84036477180-g8du4c9m1nvh7ducvvj0mkgm3dp9pfjp.apps.googleusercontent.com">
            <Router>
                <Routes>
                    <Route path="/" element={<LoginForm />} />
                    <Route path="/home" element={<Home />} />
                    <Route path="/createUser" element={<CreateUserForm />} />
                </Routes>
            </Router>
        </GoogleOAuthProvider>
    );
}

export default App;
