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
	description NVARCHAR(250) NOT NULL
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
    upload_by INT NOT NULL,
    FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
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
	location INT NOT NULL,
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
	description NVARCHAR(250) NOT NULL,
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
('Manager User', '$2a$11$1IReM3Zy6rfe7ObrWPzuZONAoB/3BxCXPFNIt/AgKCq9KXVZM0DQy', 'manager@uniexetask.com', 'https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg', '0901000002', 2, 2);

-- Thêm dữ liệu mẫu cho bảng ROLE_PERMISSION
INSERT INTO ROLE_PERMISSION (role_id, permission_id)
VALUES 
(1, 1), (1, 2), (1, 3),
(2, 1), (2, 2),
(3, 1),
(4, 1),
(5, 1);

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