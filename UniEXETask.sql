CREATE DATABASE UniEXETask

GO

USE UniEXETask

GO

-- Tạo bảng Campus
CREATE TABLE CAMPUS (
    campus_id INT PRIMARY KEY IDENTITY(1,1),
    campus_code NVARCHAR(50) NOT NULL,
    campus_name NVARCHAR(100) NOT NULL,
    location NVARCHAR(100) NOT NULL
);

-- Tạo bảng Role
CREATE TABLE ROLE (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng Permission
CREATE TABLE PERMISSION (
    permission_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng User
CREATE TABLE [USER] (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(100) NOT NULL,
    [password] NVARCHAR(255) NOT NULL,
    email NVARCHAR(100) NOT NULL UNIQUE,
    phone NVARCHAR(20),
    campus_id INT NOT NULL,
    status BIT NOT NULL DEFAULT 1,
    role_id INT NOT NULL,
    FOREIGN KEY (campus_id) REFERENCES CAMPUS(campus_id),
    FOREIGN KEY (role_id) REFERENCES ROLE(role_id)
);

-- Tạo bảng ROLE_PERMISSION
CREATE TABLE ROLE_PERMISSION (
    role_id INT NOT NULL,
    permission_id INT NOT NULL,
    PRIMARY KEY (role_id, permission_id),
    FOREIGN KEY (role_id) REFERENCES Role(role_id),
    FOREIGN KEY (permission_id) REFERENCES PERMISSION(permission_id)
);

-- Tạo bảng CHAT_GROUP
CREATE TABLE CHAT_GROUP (
    chat_group_id INT PRIMARY KEY IDENTITY(1,1),
    chatbox_name NVARCHAR(50) NOT NULL,
    created_date DATETIME DEFAULT GETDATE() NOT NULL,
	created_by INT NOT NULL,
	owner_id INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Public', 'Private')) NOT NULL,
	FOREIGN KEY (created_by) REFERENCES [USER](user_id),
	FOREIGN KEY (owner_id) REFERENCES [USER](user_id),
);

-- Tạo bảng CHAT_MESSAGE
CREATE TABLE CHAT_MESSAGE (
    message_id INT PRIMARY KEY IDENTITY(1,1),
    chat_group_id INT NOT NULL,
	user_id INT NOT NULL,
	message_content NVARCHAR(4000) NOT NULL,
	send_datetime DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (chat_group_id) REFERENCES CHAT_GROUP(chat_group_id),
    FOREIGN KEY (user_id) REFERENCES [USER](user_id)
);

-- Tạo bảng SUBJECT
CREATE TABLE SUBJECT (
    subject_id INT PRIMARY KEY IDENTITY(1,1),
    subject_code NVARCHAR(50) NOT NULL,
    subject_name DATETIME DEFAULT GETDATE() NOT NULL,
	description INT NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
);

-- Tạo bảng PROJECT
CREATE TABLE PROJECT (
    project_id INT PRIMARY KEY IDENTITY(1,1),
    topic_code NVARCHAR(10) NOT NULL,
    topic_name NVARCHAR(50) NOT NULL,
	description NVARCHAR(250) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	subject_id INT NOT NULL,
	score INT NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id),
);

-- Tạo bảng TASK
CREATE TABLE TASK (
    task_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    name NVARCHAR(50) NOT NULL,
	description NVARCHAR(250) NOT NULL,
	due_date DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
);

-- Tạo bảng LABEL
CREATE TABLE LABEL (
    label_id INT PRIMARY KEY IDENTITY(1,1),
    label_name NVARCHAR(50) NOT NULL,	
);

-- Tạo bảng PROJECT_LABEL
CREATE TABLE PROJECT_LABEL (
    project_id INT NOT NULL,
    label_id INT NOT NULL,
    PRIMARY KEY (project_id, label_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
    FOREIGN KEY (label_id) REFERENCES LABEL(label_id)
);

-- Tạo bảng REQUIREMENT
CREATE TABLE REQUIREMENT (
    requirement_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    requestor_id INT NOT NULL,
	content NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
);

-- Tạo bảng RESOURCE
CREATE TABLE RESOURCE (
    resource_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    name NVARCHAR(250) NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Status 1', 'Status 2')) NOT NULL,
	url NVARCHAR(250) NOT NULL,
	upload_by INT NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
);

-- Tạo bảng SPONSOR
CREATE TABLE SPONSOR (
    sponsor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Status 1', 'Status 2')) NOT NULL,
	investment_field NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id),
);

-- Tạo bảng PROJECT_SPONSOR
CREATE TABLE PROJECT_SPONSOR (
    project_id INT NOT NULL,
    sponsor_id INT NOT NULL,
    PRIMARY KEY (project_id, sponsor_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
    FOREIGN KEY (sponsor_id) REFERENCES SPONSOR(sponsor_id)
);

-- Tạo bảng MENTOR
CREATE TABLE MENTOR (
    mentor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	title NVARCHAR(250) NOT NULL,
	specialty NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id),
);

-- Tạo bảng PROJECT_MENTOR
CREATE TABLE PROJECT_MENTOR (
    project_id INT NOT NULL,
    mentor_id INT NOT NULL,
    PRIMARY KEY (project_id, mentor_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
    FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
);

-- Tạo bảng GROUP
CREATE TABLE [GROUP] (
    group_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
	group_name NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
);

-- Tạo bảng MEETING_SCHEDULE
CREATE TABLE MEETING_SCHEDULE (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    group_id INT NOT NULL,
    mentor_id INT NOT NULL,
	location INT NOT NULL,
	meeting_date DATETIME NOT NULL,
	duration INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Offline', 'Online')) NOT NULL,
	content NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
	FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id),
);

-- Tạo bảng STUDENT
CREATE TABLE STUDENT (
    student_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	student_code NVARCHAR(10) NOT NULL,
	major NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id),
);

-- Tạo bảng GROUP_MEMBER
CREATE TABLE GROUP_MEMBER (
    group_id INT NOT NULL,
    student_id INT NOT NULL,
	role NVARCHAR(20) CHECK (role IN ('Leader', 'Member')) NOT NULL,
    PRIMARY KEY (group_id, student_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (student_id) REFERENCES STUDENT(student_id)
);

-- Tạo bảng NOFITICATION
CREATE TABLE NOFITICATION (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
	message INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Offline', 'Online')) NOT NULL,
	created_at DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (sender_id) REFERENCES [USER](user_id),
	FOREIGN KEY (receiver_id) REFERENCES [USER](user_id),
);

-- Tạo bảng GROUP_INVITE
CREATE TABLE GROUP_INVITE (
    group_id INT NOT NULL,
    notification_id INT NOT NULL,
	inviter_id INT NOT NULL,
    invitee_id INT NOT NULL,
	created_at DATETIME DEFAULT GETDATE() NOT NULL,
    updated_at DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
    PRIMARY KEY (group_id, notification_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (notification_id) REFERENCES NOFITICATION(notification_id)
);

-- Tạo bảng EVENT
CREATE TABLE EVENT (
    event_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(250) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	location NVARCHAR(250) NOT NULL,
	reg_url NVARCHAR(250) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
);



