import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginForm from './components/Login/LoginForm';
import Home from './components/User/Home';
import CreateUserForm from './components/User/CreateUserForm';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LoginForm />} />
                <Route path="/home" element={<Home />} />
                <Route path="/createUser" element={<CreateUserForm />} />
            </Routes>
        </Router>
    );
}

export default App;

