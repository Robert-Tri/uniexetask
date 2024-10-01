import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './LoginForm.css';

const LoginForm = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');

        try {
            // Giả lập đăng nhập thành công
            console.log('Đăng nhập với:', { email, password });
            navigate('/home');
        } catch (err) {
            console.error(err);
            setError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
        }
    };

    const handleGoogleLogin = () => {
        // Xử lý đăng nhập bằng Google ở đây
        console.log('Đăng nhập bằng Google');
    };

    return (
        <div className="login-page">
            <div className="login-container">
                <h1 className="site-title">UniEXETask</h1>
                <form onSubmit={handleLogin} className="login-form">
                    <h2>Đăng Nhập</h2>
                    <div className="input-group">
                        <label htmlFor="email">Email</label>
                        <input
                            id="email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>
                    <div className="input-group">
                        <label htmlFor="password">Mật khẩu</label>
                        <input
                            id="password"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button type="submit" className="login-button">Đăng Nhập</button>
                    {error && <p className="error">{error}</p>}
                </form>
                <div className="separator">
                    <span>hoặc</span>
                </div>
                <button onClick={handleGoogleLogin} className="google-login-button">
                    <img src="/images/logo-google.png" alt="Google logo" className="google-icon" />
                    Đăng nhập bằng Google
                </button>
                <p className="register-link">
                    Chưa có tài khoản? <a href="/register">Đăng ký ngay</a>
                </p>
            </div>
        </div>
    );
};

export default LoginForm;