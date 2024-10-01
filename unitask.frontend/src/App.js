import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginForm from './components/Login/LoginForm';
import Home from './components/User/Home';
import UserForm from './components/User/UserForm';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LoginForm />} />
                <Route path="/home" element={<Home />} />
                <Route path="/createUser" element={<UserForm />} />
            </Routes>
        </Router>
    );
}

export default App;

