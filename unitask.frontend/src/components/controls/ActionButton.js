import React from 'react';
import { Button } from '@material-ui/core';

const ActionButton = ({ bgColor, textColor, onClick, children }) => {
    return (
        <Button
            style={{
                minWidth: 0,
                margin: '4px',
                backgroundColor: bgColor, // Màu nền mặc định
                color: textColor, // Màu chữ mặc định
                transition: 'background-color 0.3s, color 0.3s',
            }}
            onClick={onClick}
            onMouseEnter={(e) => {
                e.currentTarget.style.backgroundColor = textColor; // Đổi màu nền khi hover
                e.currentTarget.style.color = bgColor; // Đổi màu chữ khi hover
            }}
            onMouseLeave={(e) => {
                e.currentTarget.style.backgroundColor = bgColor; // Trở lại màu nền mặc định
                e.currentTarget.style.color = textColor; // Trở lại màu chữ mặc định
            }}
        >
            {children}
        </Button>
    );
};

export default ActionButton;
