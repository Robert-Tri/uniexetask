import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { GoogleLogin } from '@react-oauth/google';
import { API_BASE_URL } from '../../config';
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
            const response = await axios.post(`${API_BASE_URL}api/auth/login`, {
                email,
                password,
            });

            document.cookie = `AccessToken=${response.data.data.accessToken}; path=/; secure;`;
            document.cookie = `RefreshToken=${response.data.data.refreshToken}; path=/; secure;`;

            navigate('/home');
        } catch (err) {
            console.error(err);
            setError('Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin.');
        }
    };

    const handleGoogleLoginSuccess = async (credentialResponse) => {
        console.log('Credential Response:', credentialResponse); // Kiểm tra giá trị

        setError('');
        try {
            const response = await axios.post(`${API_BASE_URL}api/auth/google-login`, {
                token: credentialResponse.credential // Gửi token cho API
            });

            document.cookie = `AccessToken=${response.data.data.accessToken}; path=/; secure;`;
            document.cookie = `RefreshToken=${response.data.data.refreshToken}; path=/; secure;`;

            navigate('/home');
        } catch (err) {
            console.error(err);
            setError('Đăng nhập bằng Google thất bại.');
        }
    };

    const handleGoogleLoginError = () => {
        setError('Đăng nhập bằng Google thất bại.');
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
                <GoogleLogin
                    onSuccess={handleGoogleLoginSuccess}
                    onError={handleGoogleLoginError}
                    clientId="84036477180-g8du4c9m1nvh7ducvvj0mkgm3dp9pfjp.apps.googleusercontent.com"
                />
                <p className="register-link">
                    Chưa có tài khoản? <a href="/register">Đăng ký ngay</a>
                </p>
            </div>
        </div>
    );
};

export default LoginForm;
