import React from 'react';

const Home = () => {
    return (
        <div style={styles.container}>
            <h1 style={styles.title}>Welcome to UniEXETask</h1>
            <p style={styles.description}>This is the homepage after successful login.</p>
        </div>
    );
};

const styles = {
    container: {
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        backgroundColor: '#f0f0f0',
    },
    title: {
        fontSize: '36px',
        color: '#333',
    },
    description: {
        fontSize: '18px',
        color: '#666',
    },
};

export default Home;
