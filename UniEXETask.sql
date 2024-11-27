USE master
GO
CREATE DATABASE UniEXETask
GO
USE UniEXETask
GO

-- Tạo bảng Campus
CREATE TABLE CAMPUS (
    campus_id INT PRIMARY KEY IDENTITY(1,1),
    campus_code NVARCHAR(50) NOT NULL,
    campus_name NVARCHAR(100) NOT NULL,
    location NVARCHAR(100) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng SUBJECT
CREATE TABLE SUBJECT (
    subject_id INT PRIMARY KEY IDENTITY(1,1),
    subject_code NVARCHAR(50) NOT NULL,
    subject_name NVARCHAR(50) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng Role
CREATE TABLE ROLE (
    role_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng FEATURE
CREATE TABLE FEATURE (
    feature_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
);

-- Tạo bảng Permission
CREATE TABLE PERMISSION (
    permission_id INT PRIMARY KEY IDENTITY(1,1),
	feature_id INT NOT NULL,
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(255)
	FOREIGN KEY (feature_id) REFERENCES FEATURE(feature_id),
);

-- Tạo bảng User
CREATE TABLE [USER] (
    user_id INT PRIMARY KEY IDENTITY(1,1),
    full_name NVARCHAR(100) NOT NULL,
    [password] NVARCHAR(255),
    email NVARCHAR(100) NOT NULL UNIQUE,
    phone NVARCHAR(20),
	avatar NVARCHAR(max),
    campus_id INT NOT NULL,
    role_id INT NOT NULL,
    FOREIGN KEY (campus_id) REFERENCES CAMPUS(campus_id),
    FOREIGN KEY (role_id) REFERENCES ROLE(role_id),
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng ROLE_PERMISSION
CREATE TABLE ROLE_PERMISSION (
    role_id INT NOT NULL,
    permission_id INT NOT NULL,
    PRIMARY KEY (role_id, permission_id),
    FOREIGN KEY (role_id) REFERENCES Role(role_id),
    FOREIGN KEY (permission_id) REFERENCES PERMISSION(permission_id)
);

-- Tạo bảng MENTOR
CREATE TABLE MENTOR (
    mentor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	specialty NVARCHAR(250) NOT NULL,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id)
);

-- Tạo bảng STUDENT
CREATE TABLE STUDENT (
    student_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	lecturer_id INT NOT NULL,
    student_code NVARCHAR(10) NOT NULL UNIQUE,
    major NVARCHAR(250) NOT NULL,
	subject_id INT NOT NULL,
	isCurrentPeriod BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (user_id) REFERENCES [USER](user_id),
	FOREIGN KEY (lecturer_id) REFERENCES MENTOR(mentor_id),
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id)
);

-- Tạo bảng CHAT_GROUP
CREATE TABLE CHAT_GROUP (
    chat_group_id INT PRIMARY KEY IDENTITY(1,1),
    chat_group_name NVARCHAR(50) NOT NULL,
	chat_group_avatar nvarchar(255),
    created_date DATETIME DEFAULT GETDATE() NOT NULL,
	created_by INT NOT NULL,
	owner_id INT NOT NULL,
	group_id INT,
	latest_activity DATETIME DEFAULT GETDATE() NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Personal', 'Group')) NOT NULL,
	FOREIGN KEY (created_by) REFERENCES [USER](user_id),
	FOREIGN KEY (owner_id) REFERENCES [USER](user_id)
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

-- Tạo bảng USER_CHAT_GROUP
CREATE TABLE USER_CHAT_GROUP (
    user_id INT NOT NULL,
    chat_group_id INT NOT NULL,
    PRIMARY KEY (user_id, chat_group_id),
    FOREIGN KEY (user_id) REFERENCES [USER](user_id),
    FOREIGN KEY (chat_group_id) REFERENCES CHAT_GROUP(chat_group_id)
);

-- Tạo bảng TIMELINE
CREATE TABLE TIMELINE (
    timeline_id INT PRIMARY KEY IDENTITY(1,1),
    timeline_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(250) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	subject_id INT NOT NULL,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id)
);

-- Tạo bảng TOPIC
CREATE TABLE TOPIC (
    topic_id INT PRIMARY KEY IDENTITY(1,1),
	topic_code NVARCHAR(50) NOT NULL,
	topic_name NVARCHAR(100) NOT NULL,
	description NVARCHAR(MAX) NOT NULL
);

-- Tạo bảng GROUP
CREATE TABLE [GROUP] (
    group_id INT PRIMARY KEY IDENTITY(1,1),
	group_name NVARCHAR(250) NOT NULL,
	subject_id INT NOT NULL,
	hasMentor BIT NOT NULL,
	isCurrentPeriod BIT NOT NULL DEFAULT 1,
	status NVARCHAR(20) CHECK (status IN ('Initialized', 'Eligible', 'Approved', 'Overdue')) NOT NULL,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id),
	isDeleted BIT NOT NULL DEFAULT 0,
);

-- Tạo bảng PROJECT
CREATE TABLE PROJECT (
    project_id INT PRIMARY KEY IDENTITY(1,1),
	group_id INT NOT NULL,
	topic_id INT NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	subject_id INT NOT NULL,
	isCurrentPeriod BIT NOT NULL DEFAULT 1,
	status NVARCHAR(20) CHECK (status IN ('In_Progress', 'Completed')) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id),
	FOREIGN KEY (topic_id) REFERENCES TOPIC(topic_id),
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id)
);

-- Tạo bảng PROJECT_PROGRESS
CREATE TABLE PROJECT_PROGRESS (
    project_progress_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    progress_percentage DECIMAL(5,2) CHECK (progress_percentage >= 0 AND progress_percentage <= 100) NOT NULL DEFAULT 0,
    updated_date DATETIME NOT NULL,
    note NVARCHAR(250),
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id)
);

-- Tạo bảng TASK
CREATE TABLE TASK (
    task_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    task_name NVARCHAR(50) NOT NULL,
	description NVARCHAR(250) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Not_Started', 'In_Progress', 'Completed', 'Overdue')) NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng TASK_PROGRESS
CREATE TABLE TASK_PROGRESS (
    task_progress_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    progress_percentage DECIMAL(5,2) CHECK (progress_percentage >= 0 AND progress_percentage <= 100) NOT NULL DEFAULT 0,
    updated_date DATETIME NOT NULL,
    note NVARCHAR(250),
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (task_id) REFERENCES TASK(task_id)
);

-- Tạo bảng TASK_ASSIGN
CREATE TABLE TASK_ASSIGN (
    task_assign_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    student_id INT NOT NULL,
	assigned_date DATETIME NOT NULL,
	FOREIGN KEY (task_id) REFERENCES TASK(task_id),
	FOREIGN KEY (student_id) REFERENCES STUDENT(student_id)
);

-- Tạo bảng TASK_DETAIL
CREATE TABLE TASK_DETAIL (
    task_detail_id INT PRIMARY KEY IDENTITY(1,1),
    task_id INT NOT NULL,
    task_detail_name NVARCHAR(250) NOT NULL, -- Mô tả công việc cụ thể trong task
	progress_percentage DECIMAL(5,2) CHECK (progress_percentage >= 0 AND progress_percentage <= 100) NOT NULL DEFAULT 0,
	isCompleted BIT NOT NULL DEFAULT 0,
	isDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (task_id) REFERENCES TASK(task_id)
);

-- Tạo bảng LABEL
CREATE TABLE LABEL (
    label_id INT PRIMARY KEY IDENTITY(1,1),
    label_name NVARCHAR(50) NOT NULL
);

-- Tạo bảng PROJECT_LABEL
CREATE TABLE PROJECT_LABEL (
    project_id INT NOT NULL,
    label_id INT NOT NULL,
    PRIMARY KEY (project_id, label_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
    FOREIGN KEY (label_id) REFERENCES LABEL(label_id)
);

-- Tạo bảng DOCUMENT
CREATE TABLE DOCUMENT (
    document_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    name NVARCHAR(250) NOT NULL,
    type NVARCHAR(20) CHECK (type IN (
        'DOC',
        'DOCX',
        'XLS',      
        'XLSX',
        'PDF',      
        'TXT',      
        'JPG',      
        'JPEG',
        'PNG',
        'ZIP',      
        'RAR'
    )) NOT NULL,
    url NVARCHAR(250) NOT NULL,
	modified_by INT,
	modified_date DATETIME,
    upload_by INT NOT NULL,
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	FOREIGN KEY (modified_by) REFERENCES [USER](user_id),
	FOREIGN KEY (upload_by) REFERENCES [USER](user_id),
    isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng FUNDING
CREATE TABLE FUNDING (
    funding_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    amount_money FLOAT,
    approved_date DATETIME NOT NULL,
    document_id INT,
    status NVARCHAR(20) CHECK (status IN ('Pending', 'Approved', 'Rejected')) NOT NULL,
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	FOREIGN KEY (document_id) REFERENCES DOCUMENT(document_id)
);

-- Tạo bảng USAGE_PLAN
CREATE TABLE USAGE_PLAN (
    usage_plan_id INT PRIMARY KEY IDENTITY(1,1),
    funding_id INT NOT NULL,
    title NVARCHAR(255) NOT NULL,
    amount FLOAT NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    status NVARCHAR(20) CHECK (status IN ('Planned', 'Spent', 'Not_Achieved')) NOT NULL,
    FOREIGN KEY (funding_id) REFERENCES FUNDING(funding_id)
);

-- Tạo bảng EXPENSE_REPORT
CREATE TABLE EXPENSE_REPORT (
    expense_report_id INT PRIMARY KEY IDENTITY(1,1),
    usage_plan_id INT NOT NULL,
    spent_amount FLOAT NOT NULL,
    spent_date DATETIME NOT NULL,
    receipt_url NVARCHAR(500) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    FOREIGN KEY (usage_plan_id) REFERENCES USAGE_PLAN(usage_plan_id)
);

-- Tạo bảng MENTOR_GROUP
CREATE TABLE MENTOR_GROUP (
    group_id INT NOT NULL,
    mentor_id INT NOT NULL,
    PRIMARY KEY (group_id, mentor_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
);

-- Tạo bảng MEETING_SCHEDULE
CREATE TABLE MEETING_SCHEDULE (
    schedule_id INT PRIMARY KEY IDENTITY(1,1),
    group_id INT NOT NULL,
    mentor_id INT NOT NULL,
	location NVARCHAR(MAX) NOT NULL,
	meeting_date DATETIME NOT NULL,
	duration INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Offline', 'Online')) NOT NULL,
	content NVARCHAR(250) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
	FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
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

-- Tạo bảng NOTIFICATION
CREATE TABLE NOTIFICATION (
    notification_id INT PRIMARY KEY IDENTITY(1,1),
    sender_id INT NOT NULL,
    receiver_id INT NOT NULL,
	message NVARCHAR(250) NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Info', 'Group_Request')) NOT NULL,
	created_at DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Sent', 'Read')) NOT NULL,
	FOREIGN KEY (sender_id) REFERENCES [USER](user_id),
	FOREIGN KEY (receiver_id) REFERENCES [USER](user_id)
);

-- Tạo bảng GROUP_INVITE
CREATE TABLE GROUP_INVITE (
    group_id INT NOT NULL,
    notification_id INT NOT NULL,
	inviter_id INT NOT NULL,
    invitee_id INT NOT NULL,
	created_date DATETIME DEFAULT GETDATE() NOT NULL,
    updated_date DATETIME NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Pending', 'Accepted', 'Rejected')) NOT NULL,
    PRIMARY KEY (group_id, notification_id),
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
    FOREIGN KEY (notification_id) REFERENCES NOTIFICATION(notification_id)
);

-- Tạo bảng EVENT
CREATE TABLE WORKSHOP (
    workshop_id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	location NVARCHAR(MAX) NOT NULL,
	reg_url NVARCHAR(MAX) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Not_Started', 'In_Progress', 'Completed')) NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0
);

-- Tạo bảng MILESTONE để định nghĩa các mốc đánh giá
CREATE TABLE MILESTONE (
    milestone_id INT PRIMARY KEY IDENTITY(1,1),
    milestone_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(250),
	percentage FLOAT NOT NULL,
	subject_id INT NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
    created_date DATETIME DEFAULT GETDATE() NOT NULL,
    updated_date DATETIME,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (subject_id) REFERENCES SUBJECT(subject_id)
);

-- Tạo bảng CRITERIA
CREATE TABLE CRITERIA (
    criteria_id INT PRIMARY KEY IDENTITY(1,1),
    criteria_name NVARCHAR(250) NOT NULL,
	description NVARCHAR(250) NOT NULL,
    percentage FLOAT NOT NULL,
	milestone_id INT NOT NULL,
	created_date DATETIME DEFAULT GETDATE() NOT NULL,
    updated_date DATETIME NOT NULL,
	isDeleted BIT NOT NULL DEFAULT 0,
	FOREIGN KEY (milestone_id) REFERENCES MILESTONE(milestone_id)
);

-- Tạo bảng PROJECT_SCORE
CREATE TABLE PROJECT_SCORE (
    project_score_id INT PRIMARY KEY IDENTITY(1,1),
    criteria_id INT NOT NULL,
	project_id INT NOT NULL,
    score FLOAT NOT NULL,
	comment NVARCHAR(250) NOT NULL,
    scored_by INT NOT NULL, -- ID của người chấm điểm (mentor)
    scoring_date DATETIME DEFAULT GETDATE() NOT NULL,
	FOREIGN KEY (criteria_id) REFERENCES CRITERIA(criteria_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
	FOREIGN KEY (scored_by) REFERENCES [USER](user_id),
	CONSTRAINT CHK_ProjectScore CHECK (score >= 0 AND score <= 10)
);

-- Tạo bảng MEMBER_SCORE để lưu điểm của từng thành viên theo milestone
CREATE TABLE MEMBER_SCORE (
    member_score_id INT PRIMARY KEY IDENTITY(1,1),
    student_id INT NOT NULL,
    project_id INT NOT NULL,
    milestone_id INT NOT NULL,
    score FLOAT NOT NULL,
    comment NVARCHAR(250),
    scored_by INT NOT NULL, -- ID của người chấm điểm (mentor)
    scoring_date DATETIME DEFAULT GETDATE() NOT NULL,
    FOREIGN KEY (student_id) REFERENCES STUDENT(student_id),
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
    FOREIGN KEY (milestone_id) REFERENCES MILESTONE(milestone_id),
    FOREIGN KEY (scored_by) REFERENCES MENTOR(mentor_id),
    CONSTRAINT CHK_MemberScore CHECK (score >= 0 AND score <= 10)
);

-- Tạo bảng REFRESH_TOKEN
CREATE TABLE REFRESH_TOKEN (
    token_id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT NOT NULL,
    token NVARCHAR(MAX) NOT NULL,
    expires DATETIME NOT NULL,
	created  DATETIME DEFAULT GETDATE() NOT NULL,
	revoked  DATETIME NOT NULL,
	status BIT NOT NULL DEFAULT 1,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id)
);

-- Tạo bảng REG_TOPIC_FORM
CREATE TABLE REG_TOPIC_FORM (
    reg_topic_id INT PRIMARY KEY IDENTITY(1,1),
    group_id INT NOT NULL,
	topic_code NVARCHAR(50) NOT NULL,
	topic_name NVARCHAR(100) NOT NULL,
	description NVARCHAR(MAX) NOT NULL,
	status BIT NOT NULL,
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id)
);

-- Tạo bảng REG_MEMBER_FORM
CREATE TABLE REG_MEMBER_FORM (
    reg_member_id INT PRIMARY KEY IDENTITY(1,1),
    group_id INT NOT NULL,
	description NVARCHAR(250) NOT NULL,
	status BIT NOT NULL,
    FOREIGN KEY (group_id) REFERENCES [GROUP](group_id)
);

-- Tạo bảng TOPIC_FOR_MENTOR
CREATE TABLE TOPIC_FOR_MENTOR (
    topic_for_mentor_id INT PRIMARY KEY IDENTITY(1,1),
    mentor_id INT NOT NULL,
	topic_code NVARCHAR(50) NOT NULL,
	topic_name NVARCHAR(100) NOT NULL,
	description NVARCHAR(MAX) NOT NULL,
	isRegistered BIT NOT NULL,
    FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id)
);

-- SS: 
-- SE:
-- AI:


-- Thêm dữ liệu mẫu cho bảng Campus
INSERT INTO CAMPUS (campus_code, campus_name, location)
VALUES 
('FPT-HN', 'FPT Ha Noi', 'Ha Noi'),
('FPT-HCM', 'FPT Ho Chi Minh', 'Ho Chi Minh'),
('FPT-DN', 'FPT Da Nang', 'Da Nang');

-- Thêm dữ liệu mẫu cho bảng SUBJECT
INSERT INTO SUBJECT (subject_code, subject_name)
VALUES 
('EXE101', 'Entrepreneurship Basics'),
('EXE102', 'Advanced Entrepreneurship');

-- Thêm dữ liệu mẫu cho bảng Role
INSERT INTO ROLE (name, description)
VALUES 
('Admin', 'Administrator with full access'),
('Manager', 'Manager with project management privileges'),
('Student', 'Student participating in projects'),
('Mentor', 'Mentor providing guidance to projects'),
('Sponsor', 'Sponsor investing in projects');

-- Thêm dữ liệu mẫu cho bảng feature
INSERT INTO FEATURE (name, description)
VALUES 
('User Management', 'Feature to manage (view, create, update, delete, import) users'),
('Project Management', 'Feature to manage (view, create, update, delete) projects'),
('Event Management', 'Feature to manage (view, create, update, delete) events'),
('Meeting Schedule Management', 'Feature to manage (view, create, update, delete) meeting schedules in the project'),
('Group Management', 'Feature to manage (view, create, update, delete) group'),
('Resource Management', 'Feature to manage (view, upload, update, delete, download) resources in the project');

-- Thêm dữ liệu mẫu cho bảng Permission
INSERT INTO PERMISSION (feature_id, name, description)
VALUES 
(1, 'view_user', 'Permission to view users'),
(1, 'create_user', 'Permission to create users'),
(1, 'edit_user', 'Permission to edit users'),
(1, 'delete_user', 'Permission to delete users'),
(1, 'import_user', 'Permission to import users from ecel file'),
(2, 'view_project', 'Permission to view projects'),
(2, 'create_project', 'Permission to create projects'),
(2, 'edit_project', 'Permission to edit projects'),
(2, 'delete_project', 'Permission to delete projects'),
(3, 'view_event', 'Permission to view events'),
(3, 'create_event', 'Permission to create events'),
(3, 'edit_event', 'Permission to edit events'),
(3, 'delete_event', 'Permission to delete events'),
(4, 'view_meeting_schedule', 'Permission to view meeting schedules'),
(4, 'create_meeting_schedule', 'Permission to create meeting schedules'),
(4, 'edit_meeting_schedule', 'Permission to edit meeting schedules'),
(4, 'delete_meeting_schedule', 'Permission to delete meeting schedules'),
(5, 'view_group', 'Permission to view group'),
(5, 'create_group', 'Permission to create group'),
(5, 'edit_group', 'Permission to edit group'),
(5, 'delete_group', 'Permission to delete group'),
(6, 'view_resource', 'Permission to view resources'),
(6, 'upload_resource', 'Permission to create resources'),
(6, 'edit_resource', 'Permission to edit resources'),
(6, 'delete_resource', 'Permission to delete resources'),
(6, 'download_resource', 'Permission to delete resources');

-- Thêm dữ liệu mẫu cho bảng User
-- Default password: Uniexetask123456
INSERT INTO [USER] (full_name, [password], email, avatar, phone, campus_id, role_id)
VALUES 
('Admin User', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'admin@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000001', 1, 1),
('Manager User', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'manager@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000002', 2, 2),
('Student User 1', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student1@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000003', 3, 3),
('Mentor User 1', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor1@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000004', 1, 4),
('Sponsor User 1', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'sponsor1@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000005', 2, 5),
('Student User 2', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student2@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000006', 1, 3),
('Mentor User 2', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor2@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000007', 2, 4),
('Sponsor User 2', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'sponsor2@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000008', 1, 5),
('Student User 3', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student3@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000009', 2, 3),
('Mentor User 3', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor3@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000010', 3, 4),
('Sponsor User 3', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'sponsor3@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000011', 3, 5),
(N'Nguyễn Huỳnh Đức Trí', NULL, 'trinhdse162014@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0867892130', 1, 3),
(N'Phan Song Thảo', NULL, 'thaopsse162032@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0837250452', 1, 3),
(N'Lê Hòa Bình', NULL, 'binhlhse162087@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0913926749', 1, 3),
(N'Trần Hồng Hưng', NULL, 'hungthse162056@fpt.edu.vn', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0374312384', 1, 3),
-- 5 Mentors (role_id = 4)
('Mentor User 4', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor4@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000012', 1, 4),
('Mentor User 5', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor5@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000013', 2, 4),
('Mentor User 6', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor6@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000014', 3, 4),
('Mentor User 7', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor7@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000015', 1, 4),
('Mentor User 8', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'mentor8@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000016', 2, 4),
-- 30 Students (role_id = 3)
('Student User 4', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student4@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000017', 1, 3),
('Student User 5', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student5@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000018', 2, 3),
('Student User 6', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student6@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000019', 3, 3),
('Student User 7', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student7@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000020', 1, 3),
('Student User 8', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student8@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000021', 2, 3),
('Student User 9', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student9@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000022', 3, 3),
('Student User 10', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student10@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000023', 1, 3),
('Student User 11', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student11@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000024', 2, 3),
('Student User 12', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student12@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000025', 3, 3),
('Student User 13', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student13@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000026', 1, 3),
('Student User 14', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student14@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000027', 2, 3),
('Student User 15', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student15@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000028', 3, 3),
('Student User 16', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student16@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000029', 1, 3),
('Student User 17', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student17@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000030', 2, 3),
('Student User 18', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student18@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000031', 3, 3),
('Student User 19', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student19@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000032', 1, 3),
('Student User 20', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student20@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000033', 2, 3),
('Student User 21', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student21@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000034', 3, 3),
('Student User 22', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student22@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000035', 1, 3),
('Student User 23', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student23@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000036', 2, 3),
('Student User 24', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student24@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000037', 3, 3),
('Student User 25', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student25@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000038', 1, 3),
('Student User 26', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student26@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000039', 2, 3),
('Student User 27', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student27@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000040', 3, 3),
('Student User 28', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student28@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000041', 1, 3),
('Student User 29', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student29@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000042', 2, 3),
('Student User 30', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student30@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000043', 3, 3),
('Student User 31', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student31@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000044', 1, 3),
('Student User 32', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student32@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000045', 2, 3),
('Student User 33', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'student33@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000046', 3, 3);

-- Thêm dữ liệu mẫu cho bảng MENTOR
INSERT INTO MENTOR (user_id, specialty)
VALUES 
(4, 'Renewable Energy'),
(7, 'Urban Planning'),
(10, 'Data Science'),
(16, 'Artificial Intelligence'),
(17, 'Blockchain Technology'),
(18, 'Internet of Things'),
(19, 'Cybersecurity'),
(20, 'Renewable Energy');

-- Thêm dữ liệu mẫu cho bảng STUDENT
INSERT INTO STUDENT (user_id, lecturer_id, student_code, major, subject_id, isCurrentPeriod)
VALUES 
(3, 1,'ST12345', 'Computer Science', 1, 1),
(6, 1,'ST67890', 'Information Technology', 1, 1),
(9, 2,'SS162981', 'Financial Economics', 1, 1),
(12, 2,'SE162014', 'Software Engineering', 1, 1),
(13, 1,'SE162032', 'Software Engineering', 1, 1),
(14, 1,'SE162087', 'Software Engineering', 1, 1),
(15, 2,'SE162056', 'Software Engineering', 1, 1),
(21, 3,'ST100004', 'Software Engineering', 1, 1),
(22, 4,'ST100005', 'Software Engineering', 1, 1),
(23, 5,'ST100006', 'Software Engineering', 1, 1),
(24, 6,'ST100007', 'Software Engineering', 1, 1),
(25, 7,'ST100008', 'Software Engineering', 1, 1),
(26, 8,'ST100009', 'Software Engineering', 1, 1),
(27, 8,'ST100010', 'Software Engineering', 1, 1),
(28, 8,'ST100011', 'Software Engineering', 1, 1),
(29, 1,'ST100012', 'Software Engineering', 1, 1),
(30, 1,'ST100013', 'Software Engineering', 1, 1),
(31, 1,'ST100014', 'Software Engineering', 1, 1),
(32, 2,'ST100015', 'Software Engineering', 1, 1),
(33, 2,'ST100016', 'Software Engineering', 1, 1),
(34, 2,'ST100017', 'Software Engineering', 2, 1),
(35, 3,'ST100018', 'Software Engineering', 2, 1),
(36, 3,'ST100019', 'Software Engineering', 2, 1),
(37, 3,'ST100020', 'Software Engineering', 2, 1),
(38, 4,'ST100021', 'Software Engineering', 2, 1),
(39, 4,'ST100022', 'Software Engineering', 2, 1),
(40, 4,'ST100023', 'Software Engineering', 2, 1),
(41, 5,'ST100024', 'Software Engineering', 2, 1),
(42, 5,'ST100025', 'Software Engineering', 2, 1),
(43, 5,'ST100026', 'Software Engineering', 2, 1),
(44, 6,'ST100027', 'Software Engineering', 2, 1),
(45, 6,'ST100028', 'Software Engineering', 2, 1),
(46, 6,'ST100029', 'Software Engineering', 2, 1),
(47, 7,'ST100030', 'Software Engineering', 2, 1),
(48, 7,'ST100031', 'Software Engineering', 2, 1),
(49, 7,'ST100032', 'Software Engineering', 2, 1),
(50, 8,'ST100033', 'Software Engineering', 2, 1);

-- Thêm dữ liệu mẫu cho bảng ROLE_PERMISSION
INSERT INTO ROLE_PERMISSION (role_id, permission_id)
VALUES 
(1, 1), (1, 2), (1, 3),
(2, 1), (2, 2),
(3, 1),
(4, 1),
(5, 1);

-- Thêm dữ liệu mẫu cho bảng CHAT_GROUP
INSERT INTO CHAT_GROUP (chat_group_name, chat_group_avatar, created_by, owner_id, group_id, type)
VALUES 
('Green Energy Team', null, 1, 1, 1, 'Group'),
('Smart City Team', null, 2, 2, 2, 'Group'),
('Admin User', null, 2, 2, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal'),
('Admin User', null, 1, 1, null, 'Personal');

-- Thêm dữ liệu mẫu cho bảng CHAT_GROUP
INSERT INTO USER_CHAT_GROUP (user_id, chat_group_id)
VALUES 
(2, 1),
(30, 1),
(3, 2),
(1, 4),
(3, 4),
(1, 5),
(4, 5),
(1, 6),
(5, 6),
(1, 7),
(6, 7),
(1, 8),
(7, 8),
(1, 9),
(8, 9),
(1, 10),
(9, 10),
(1, 11),
(10, 11),
(1, 12),
(11, 12),
(1, 13),
(12, 13),
(1, 14),
(13, 14),
(1, 15),
(14, 15),
(1, 16),
(15, 16),
(1, 17),
(16, 17),
(1, 18),
(17, 18);

-- Thêm dữ liệu mẫu cho bảng CHAT_MESSAGE
INSERT INTO CHAT_MESSAGE (chat_group_id, user_id, message_content)
VALUES 
(1, 1, 'Welcome to the Admin Group!'),
(2, 2, 'Project discussions start here.'),
(3, 1, 'Hello guy.'),
(3, 2, 'Hello admin.');

-- Thêm dữ liệu mẫu cho bảng GROUP
INSERT INTO [GROUP] (group_name, subject_id, hasMentor, status, isCurrentPeriod)
VALUES 
('Green Energy Team', 1, 0, 'Initialized', 1),
('Smart City Team', 2, 1, 'Approved', 1),
('UniEXETask', 1, 1, 'Approved', 1),
('AI Innovation Team', 1, 0, 'Eligible', 1),
('Blockchain Pioneers', 2, 1, 'Initialized', 1),
('IoT Solutions', 1, 1, 'Eligible', 1),
('Cybersecurity Guardians', 1, 1, 'Approved', 1),
('Data Science Explorers', 1, 1, 'Overdue', 1),
('Cloud Computing Experts', 2, 1, 'Initialized', 1),
('ML Research Group', 1, 1, 'Approved', 0),
('Group Test', 2, 0, 'Initialized', 1);

-- Thêm dữ liệu mẫu cho bảng TOPIC
INSERT INTO TOPIC (topic_code, topic_name, description)
VALUES 
('TP001', 'Green Energy', 'Research on renewable energy'),
('TP002', 'Smart City', 'Building smart city systems'),
('TP003', 'Smart Home 10', 'Building smart home systems 10');

-- Thêm dữ liệu mẫu cho bảng PROJECT
INSERT INTO PROJECT (group_id, topic_id, start_date, end_date, subject_id, status, isCurrentPeriod)
VALUES 
(3, 1, '2024-09-01', '2025-01-01', 1, 'In_Progress', 1),
(4, 2, '2024-09-01', '2025-02-01', 2, 'In_Progress', 1),
(10, 3, '2024-05-01', '2025-08-01', 2, 'Completed', 0);

-- Thêm dữ liệu mẫu cho bảng TASK
INSERT INTO TASK (project_id, task_name, description, start_date, end_date, status)
VALUES 
(1, 'Research Phase', 'Complete the research phase of the project', '2024-11-01', '2024-12-01', 'Not_Started'),
(1, 'Project Initiation', 'Define project scope and objectives', '2024-09-01', '2024-09-15', 'Completed'),
(1, 'Requirements Gathering', 'Collect and document project requirements', '2024-09-16', '2024-10-15', 'Completed'),
(1, 'System Design', 'Create system architecture and design documents', '2024-10-08', '2024-10-23', 'Overdue'),
(1, 'Database Design', 'Design database schema and relationships', '2024-10-16', '2024-11-15', 'In_Progress'),
(1, 'UI/UX Design', 'Create user interface mockups and prototypes', '2024-10-17', '2024-11-12', 'In_Progress'),
(1, 'Backend Development', 'Implement server-side logic and APIs', '2024-11-16', '2024-12-31', 'Not_Started'),
(1, 'Frontend Development', 'Implement client-side user interface', '2024-11-16', '2024-12-31', 'Not_Started'),
(2, 'Integration and Testing', 'Integrate components and perform system testing', '2024-10-09', '2024-11-02', 'In_Progress'),
(2, 'User Acceptance Testing', 'Conduct UAT with stakeholders', '2024-11-06', '2024-11-28', 'Not_Started'),
(2, 'Prototype', 'Build a prototype for the smart city project', '2024-12-01', '2024-12-15', 'Not_Started'),
(2, 'Deployment and Documentation', 'Deploy the system and create user documentation', '2024-10-01', '2024-10-10', 'Completed'),
(3, 'Integration and Testing 10', 'Integrate components and perform system testing 10', '2024-05-09', '2024-06-02', 'Completed'),
(3, 'User Acceptance Testing 10', 'Conduct UAT with stakeholders 10', '2024-06-06', '2024-06-28', 'Completed'),
(3, 'Prototype 10', 'Build a prototype for the smart city project 10', '2024-07-01', '2024-07-15', 'Completed'),
(3, 'Deployment and Documentation 10', 'Deploy the system and create user documentation 10', '2024-07-01', '2024-07-28', 'Completed');

-- Thêm dữ liệu mẫu cho bảng PROJECT_PROGRESS
INSERT INTO PROJECT_PROGRESS (project_id, progress_percentage, updated_date, note, isDeleted)
VALUES 
(1, 20.00, '2024-09-15', 'Project scope defined', 1),
(1, 40.00, '2024-10-15', 'Requirements collected and documented', 1),
(1, 50.00, '2024-10-23', 'System design initiated', 1),
(1, 60.00, '2024-11-01', 'Research phase started', 0),
(2, 30.00, '2024-10-02', 'Initial components integrated', 1),
(2, 50.00, '2024-10-09', 'System testing in progress', 0),
(3, 100.00, '2024-07-28', 'System testing in progress 10', 0);

-- Thêm dữ liệu mẫu cho bảng TASK_PROGRESS
INSERT INTO TASK_PROGRESS (task_id, progress_percentage, updated_date, note)
VALUES 
(1, 0.00, '2024-10-30', 'Not started yet'),
(2, 100.00, '2024-09-15', 'Project initiation completed successfully'),
(3, 100.00, '2024-10-15', 'Requirements gathering completed'),
(4, 50.00, '2024-10-23', 'System design halfway completed, facing some delays'),
(5, 25.00, '2024-11-01', 'Initial database schema designed'),
(6, 30.00, '2024-11-03', 'UI mockups under review'),
(7, 0.00, '2024-10-30', 'Not started yet'),
(8, 0.00, '2024-10-30', 'Not started yet'),
(9, 40.00, '2024-10-25', 'Integration in progress, some issues encountered'),
(10, 0.00, '2024-11-05', 'Not started yet'),
(11, 0.00, '2024-12-01', 'Not started yet'),
(12, 100.00, '2024-10-10', 'Deployment and documentation completed successfully'),
(13, 100.00, '2024-06-02', 'Integration in progress, some issues encountered 10'),
(14, 100.00, '2024-06-28', 'Not started yet 10'),
(15, 100.00, '2024-07-15', 'Not started yet 10'),
(16, 100.00, '2024-07-28', 'Deployment and documentation completed successfully 10');

-- Thêm dữ liệu mẫu cho bảng TASK
INSERT INTO TASK_ASSIGN(task_id, student_id, assigned_date)
VALUES 
(1, 4, '2024-11-01'),
(1, 1, '2024-11-01'),
(2, 5, '2024-12-01'),
(3, 4, '2024-11-01'),
(3, 6, '2024-11-01'),
(3, 7, '2024-11-01'),
(4, 5, '2024-11-01'),
(4, 7, '2024-11-01'),
(5, 4, '2024-11-01'),
(5, 1, '2024-11-01'),
(6, 5, '2024-11-01'),
(6, 6, '2024-11-01'),
(6, 7, '2024-11-01'),
(7, 6, '2024-11-01'),
(8, 4, '2024-11-01'),
(8, 1, '2024-11-01'),
(9, 8, '2024-11-01'),
(10, 9, '2024-11-01'),
(11, 8, '2024-11-01'),
(11, 10, '2024-11-01'),
(11, 11, '2024-11-01'),
(12, 9, '2024-11-01'),
(12, 11, '2024-11-01'),

(13, 32, '2024-05-05'),
(14, 33, '2024-05-05'),
(14, 33, '2024-05-05'),
(15, 32, '2024-05-05'),
(15, 34, '2024-05-05'),
(15, 35, '2024-05-05'),
(16, 36, '2024-05-05'),
(16, 37, '2024-05-05');

-- Thêm dữ liệu mẫu cho bảng TASK_DETAIL
INSERT INTO TASK_DETAIL (task_id, task_detail_name, progress_percentage, isCompleted, isDeleted)
VALUES 
-- Task 1: 'Research Phase'
(1, N'Nghiên cứu tài liệu và công nghệ liên quan', 80.00, 1, 0),
(1, N'Phân tích yêu cầu về nghiên cứu', 20.00, 0, 0),

-- Task 2: 'Project Initiation'
(2, N'Xác định phạm vi dự án', 40.00, 1, 0),
(2, N'Phân bổ nguồn lực và lập kế hoạch', 30.00, 1, 0),
(2, N'Báo cáo', 30.00, 0, 0),

-- Task 3: 'Requirements Gathering'
(3, N'Thu thập yêu cầu từ các bên liên quan', 60.00, 1, 0),
(3, N'Phân tích và tài liệu hóa yêu cầu', 40.00, 1, 0),

-- Task 4: 'System Design'
(4, N'Thiết kế giao diện người dùng (UI)', 45.00, 1, 0),
(4, N'Thiết kế kiến trúc hệ thống', 35.00, 0, 0),
(4, N'Báo cáo 4', 20.00, 0, 0),

-- Task 5: 'Database Design'
(5, N'Thiết kế cơ sở dữ liệu ban đầu', 40.00, 0, 0),
(5, N'Tối ưu hóa cơ sở dữ liệu', 60.00, 0, 0),

-- Task 6: 'UI/UX Design'
(6, N'Tạo mockups cho trang chủ', 30.00, 0, 0),
(6, N'Review và phản hồi từ đội ngũ thiết kế', 70.00, 1, 0),

-- Task 7: 'Backend Development'
(7, N'Phát triển API cho người dùng', 55.00, 0, 0),
(7, N'Phát triển API cho quản lý dự án', 45.00, 1, 0),

-- Task 8: 'Frontend Development'
(8, N'Phát triển giao diện cho đăng nhập', 38.00, 1, 0),
(8, N'Phát triển giao diện cho quản lý dự án', 62.00, 1, 0),

-- Task 9: 'Integration and Testing'
(9, N'Tích hợp các thành phần hệ thống', 50.00, 1, 0),
(9, N'Kiểm thử hệ thống tổng thể', 50.00, 1, 0),

-- Task 10: 'User Acceptance Testing'
(10, N'Chuẩn bị môi trường kiểm thử UAT', 67.00, 1, 0),
(10, N'Thực hiện kiểm thử UAT', 33.00, 0, 0),

-- Task 11: 'Prototype'
(11, N'Xây dựng nguyên mẫu ban đầu', 70.00, 0, 0),
(11, N'Kiểm thử và hoàn thiện nguyên mẫu', 30.00, 0, 0),

-- Task 12: 'Deployment and Documentation'
(12, N'Triển khai hệ thống trên môi trường sản xuất', 90.00, 0, 0),
(12, N'Hoàn thành tài liệu hướng dẫn sử dụng', 10.00, 0, 0),

-- Task 13: 'Integration and Testing'
(13, N'Tích hợp các thành phần hệ thống', 50.00, 1, 0),
(13, N'Kiểm thử hệ thống tổng thể', 50.00, 1, 0),

-- Task 14: 'User Acceptance Testing'
(14, N'Chuẩn bị môi trường kiểm thử UAT', 67.00, 1, 0),
(14, N'Thực hiện kiểm thử UAT', 33.00, 1, 0),

-- Task 15: 'Prototype'
(15, N'Xây dựng nguyên mẫu ban đầu', 70.00, 1, 0),
(15, N'Kiểm thử và hoàn thiện nguyên mẫu', 30.00, 1, 0),

-- Task 16: 'Deployment and Documentation'
(16, N'Triển khai hệ thống trên môi trường sản xuất', 90.00, 1, 0),
(16, N'Hoàn thành tài liệu hướng dẫn sử dụng', 10.00, 1, 0);

-- Thêm dữ liệu mẫu cho bảng LABEL
INSERT INTO LABEL (label_name)
VALUES 
('Renewable Energy'), 
('Smart City');

-- Thêm dữ liệu mẫu cho bảng PROJECT_LABEL
INSERT INTO PROJECT_LABEL (project_id, label_id)
VALUES 
(1, 1), -- Green Energy project with 'Renewable Energy' label
(2, 2); -- Smart City project with 'Smart City' label

-- Thêm dữ liệu mẫu cho bảng RESOURCE
INSERT INTO DOCUMENT(project_id, name, type, url, upload_by)
VALUES 
(1, 'Energy Research Report', 'PDF', 'http://example.com/energy_report.pdf', 1),
(2, 'Smart City Plan', 'PDF', 'http://example.com/smart_city_plan.pdf', 2);

-- Thêm dữ liệu mẫu cho bảng PROJECT_MENTOR
INSERT INTO MENTOR_GROUP (group_id, mentor_id)
VALUES 
(2, 1), 
(3, 2),
(5, 3), 
(6, 4), 
(7, 1), 
(8, 1), 
(9, 5),
(10, 1);

-- Thêm dữ liệu mẫu cho bảng MEETING_SCHEDULE
INSERT INTO MEETING_SCHEDULE (group_id, mentor_id, location, meeting_date, duration, type, content)
VALUES 
(1, 1, 1, '2024-10-01', 60, 'Offline', 'Discussing project progress'),
(2, 2, 2, '2024-11-01', 90, 'Online', 'Planning prototype development');

-- Thêm dữ liệu mẫu cho bảng GROUP_MEMBER
INSERT INTO GROUP_MEMBER (group_id, student_id, role)
VALUES 
(1, 2, 'Leader'), 
(1, 30, 'Member'),
(2, 3, 'Leader'), 
(3, 4, 'Leader'),
(3, 5, 'Member'),
(3, 6, 'Member'),
(3, 7, 'Member'),
(3, 1, 'Member'),
(4, 8, 'Leader'),
(4, 9, 'Member'),
(4, 10, 'Member'),
(4, 11, 'Member'),
(5, 13, 'Leader'),
(5, 12, 'Member'),
(5, 18, 'Member'),
(5, 19, 'Member'),
(6, 14, 'Leader'),
(6, 15, 'Member'),
(6, 16, 'Member'),
(7, 20, 'Leader'),
(7, 28, 'Member'),
(7, 25, 'Member'),
(8, 22, 'Leader'),
(8, 21, 'Member'),
(8, 23, 'Member'),
(9, 27, 'Leader'),
(9, 26, 'Member'),
(9, 29, 'Member'),
(10, 32, 'Leader'),
(10, 33, 'Member'),
(10, 34, 'Member'),
(10, 35, 'Member'),
(10, 36, 'Member'),
(10, 37, 'Member'),
(11, 33, 'Leader'),
(11, 31, 'Member');

-- Thêm dữ liệu mẫu cho bảng NOTIFICATION
INSERT INTO NOTIFICATION (sender_id, receiver_id, message, type, created_at, status)
VALUES 
(1, 2, 'Project update required', 'Group_Request', GETDATE(), 'Sent'),
(2, 3, 'Meeting reminder', 'Group_Request', GETDATE(), 'Read');

-- Thêm dữ liệu mẫu cho bảng GROUP_INVITE
INSERT INTO GROUP_INVITE (group_id, notification_id, inviter_id, invitee_id, created_date, updated_date, status)
VALUES 
(1, 1, 1, 2, GETDATE(), GETDATE(), 'Pending'),
(2, 2, 2, 3, GETDATE(), GETDATE(), 'Accepted');

-- Thêm dữ liệu mẫu cho bảng WORKSHOP
INSERT INTO WORKSHOP (name, description, start_date, end_date, location, reg_url, status)
VALUES 
('Innovation Workshop', 'Workshop on innovation and entrepreneurship', '2024-10-10', '2024-10-12', N'FPT Hà Nội', 'http://example.com/register', 'Completed'),
('Tech Expo', 'Exhibition on smart city technologies', '2024-12-01', '2024-12-03', N'FPT Hồ Chí Minh', 'http://example.com/register', 'Not_Started');

INSERT INTO REG_MEMBER_FORM (group_id, description, status)
VALUES 
    (1, N'Tôi cần tìm 1 FE có đủ kỹ năng để làm việc nhóm', 1),
    (2, N'Cần tìm 1 FE và 1 BE có thể làm việc từ xa', 1),
    (3, N'Nhóm muốn bổ sung 1 bạn BE và 1 bạn FE cho dự án mới', 1),
    (4, N'Nhóm đang cần thêm 1 FE với kinh nghiệm 2 năm', 1),
    (5, N'Chúng tôi tìm kiếm 1 BE giỏi về Node.js', 1),
    (6, N'Nhóm cần bổ sung thêm 2 người: 1 FE và 1 BE', 1),
    (7, N'Tìm 1 bạn FE biết sử dụng React', 1),
    (8, N'Tìm kiếm 1 BE có kinh nghiệm với Python', 1),
    (9, N'Nhóm cần thêm 1 FE để hoàn thành dự án', 1),
    (10, N'Chúng tôi tìm kiếm 1 bạn BE để hỗ trợ phần backend', 1);

INSERT INTO MILESTONE (milestone_name, description, percentage, subject_id, start_date, end_date, created_date, updated_date, isDeleted)
VALUES
-- Milestone 1: Project Initiation Phase
('Project Initiation', 'Complete an overview plan and define entrepreneurship objectives', 15, 1, '2024-01-01', '2024-01-15', GETDATE(), NULL, 0),

-- Milestone 2: Market Research
('Market Research', 'Analyze customer needs and conduct market surveys', 20, 1, '2024-01-16', '2024-01-30', GETDATE(), NULL, 0),

-- Milestone 3: Business Plan Development
('Business Plan Development', 'Complete a detailed business plan', 25, 1, '2024-02-01', '2024-02-15', GETDATE(), NULL, 0),

-- Milestone 4: Prototyping
('Prototyping', 'Design and test a prototype of the product/service', 20, 1, '2024-02-16', '2024-03-01', GETDATE(), NULL, 0),

-- Milestone 5: Evaluation and Refinement
('Evaluation and Refinement', 'Collect feedback and improve based on real-world testing', 15, 1, '2024-03-02', '2024-03-15', GETDATE(), NULL, 0),

-- Milestone 6: Final Presentation
('Final Presentation', 'Prepare and present the project outcomes to a panel', 5, 1, '2024-03-16', '2024-03-20', GETDATE(), NULL, 0);
INSERT INTO MILESTONE (milestone_name, description, percentage, subject_id, start_date, end_date, created_date, updated_date, isDeleted)
VALUES
-- Milestone 1: Advanced Market Analysis
('Advanced Market Analysis', 'Conduct in-depth market research with segmentation and competitive analysis', 15, 2, '2024-04-01', '2024-04-15', GETDATE(), NULL, 0),

-- Milestone 2: Financial Planning and Funding Strategy
('Financial Planning & Funding Strategy', 'Develop financial projections and identify funding opportunities', 20, 2, '2024-04-16', '2024-04-30', GETDATE(), NULL, 0),

-- Milestone 3: Building a Scalable Model
('Building a Scalable Model', 'Design scalable processes and identify key resources for growth', 25, 2, '2024-05-01', '2024-05-15', GETDATE(), NULL, 0),

-- Milestone 4: Product Refinement and User Feedback
('Product Refinement', 'Incorporate user feedback to improve product/service offerings', 15, 2, '2024-05-16', '2024-05-31', GETDATE(), NULL, 0),

-- Milestone 5: Legal and Compliance
('Legal and Compliance', 'Ensure the business adheres to legal standards and secures necessary certifications', 10, 2, '2024-06-01', '2024-06-10', GETDATE(), NULL, 0),

-- Milestone 6: Strategic Presentation for Investors
('Investor Pitch', 'Prepare and present a strategic pitch to potential investors', 15, 2, '2024-06-11', '2024-06-20', GETDATE(), NULL, 0);

INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Idea Validation', 'Evaluate the feasibility of the business idea based on market trends', 30, 1, GETDATE(), GETDATE(), 0),
('Market Research Report', 'Provide a detailed report on the target market', 40, 1, GETDATE(), GETDATE(), 0),
('SWOT Analysis', 'Complete a comprehensive SWOT analysis for the business', 30, 1, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Executive Summary', 'Summarize the core idea of the business in a concise manner', 25, 2, GETDATE(), GETDATE(), 0),
('Marketing Plan', 'Develop a strategic marketing plan', 35, 2, GETDATE(), GETDATE(), 0),
('Financial Projection', 'Create accurate financial projections for 1-3 years', 25, 2, GETDATE(), GETDATE(), 0),
('Risk Management Plan', 'Outline strategies to handle potential risks', 15, 2, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Role Assignment', 'Assign specific roles and responsibilities to team members', 30, 3, GETDATE(), GETDATE(), 0),
('Team Structure Design', 'Develop an effective team structure for the business', 30, 3, GETDATE(), GETDATE(), 0),
('Conflict Resolution Strategy', 'Create a strategy for managing team conflicts', 40, 3, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Prototype Design', 'Create a functional prototype of the product/service', 40, 4, GETDATE(), GETDATE(), 0),
('User Feedback', 'Collect and analyze feedback from initial users', 30, 4, GETDATE(), GETDATE(), 0),
('Iteration Plan', 'Plan for improving the prototype based on feedback', 30, 4, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Segment Analysis', 'Identify and analyze market segments', 30, 5, GETDATE(), GETDATE(), 0),
('Competitor Analysis', 'Evaluate competitors to identify unique selling points', 40, 5, GETDATE(), GETDATE(), 0),
('Trend Projection', 'Analyze market trends and their potential impact', 30, 5, GETDATE(), GETDATE(), 0);
INSERT INTO CRITERIA (criteria_name, description, percentage, milestone_id, created_date, updated_date, isDeleted)
VALUES
('Presentation Quality', 'Deliver a well-structured and engaging presentation', 40, 6, GETDATE(), GETDATE(), 0),
('Business Model Explanation', 'Clearly articulate the business model and its scalability', 35, 6, GETDATE(), GETDATE(), 0),
('Investor Q&A', 'Respond effectively to investor questions and concerns', 25, 6, GETDATE(), GETDATE(), 0);

INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Current Term Duration EXE101', N'Current Term Duration EXE101', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Current Term Duration EXE201', N'Current Term Duration EXE101', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Group EXE101', N'Finalize Group EXE101', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-15T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Group EXE201', N'Finalize Group EXE201', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 2)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Mentor EXE101', N'Finalize Mentor EXE201', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 1)
INSERT [dbo].[TIMELINE] ( [timeline_name], [description], [start_date], [end_date], [subject_id]) VALUES (N'Finalize Mentor EXE201', N'Finalize Mentor EXE201', CAST(N'2024-11-25T00:00:00.000' AS DateTime), CAST(N'2024-11-25T00:00:00.000' AS DateTime), 2)