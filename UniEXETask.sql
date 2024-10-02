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
    subject_name NVARCHAR(50) NOT NULL,
	description NVARCHAR(255) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
);

-- Tạo bảng PROJECT
CREATE TABLE PROJECT (
    project_id INT PRIMARY KEY IDENTITY(1,1),
    topic_code NVARCHAR(10) NOT NULL,
    topic_name NVARCHAR(50) NOT NULL,
	description NVARCHAR(255) NOT NULL,
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
	description NVARCHAR(255) NOT NULL,
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
	content NVARCHAR(255) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
);

-- Tạo bảng RESOURCE
CREATE TABLE RESOURCE (
    resource_id INT PRIMARY KEY IDENTITY(1,1),
    project_id INT NOT NULL,
    name NVARCHAR(255) NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Status 1', 'Status 2')) NOT NULL,
	url NVARCHAR(255) NOT NULL,
	upload_by INT NOT NULL,
	FOREIGN KEY (project_id) REFERENCES PROJECT(project_id),
);

-- Tạo bảng SPONSOR
CREATE TABLE SPONSOR (
    sponsor_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	type NVARCHAR(20) CHECK (type IN ('Status 1', 'Status 2')) NOT NULL,
	investment_field NVARCHAR(255) NOT NULL,
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
	title NVARCHAR(255) NOT NULL,
	specialty NVARCHAR(255) NOT NULL,
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
	group_name NVARCHAR(255) NOT NULL,
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
	content NVARCHAR(255) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
	FOREIGN KEY (group_id) REFERENCES [GROUP](group_id),
	FOREIGN KEY (mentor_id) REFERENCES MENTOR(mentor_id),
);

-- Tạo bảng STUDENT
CREATE TABLE STUDENT (
    student_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
	student_code NVARCHAR(10) NOT NULL,
	major NVARCHAR(255) NOT NULL,
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
	message NVARCHAR(255) NOT NULL,
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
    description NVARCHAR(255) NOT NULL,
	start_date DATETIME NOT NULL,
	end_date DATETIME NOT NULL,
	location NVARCHAR(255) NOT NULL,
	reg_url NVARCHAR(255) NOT NULL,
	status NVARCHAR(20) CHECK (status IN ('Status 1', 'Status 2')) NOT NULL,
);

-- Tạo bảng REFRESH_TOKEN
CREATE TABLE REFRESH_TOKEN (
    token_id INT PRIMARY KEY IDENTITY(1,1),
	user_id INT NOT NULL,
    token NVARCHAR(50) NOT NULL,
    expires DATETIME NOT NULL,
	created  DATETIME DEFAULT GETDATE() NOT NULL,
	revoked  DATETIME NOT NULL,
	status BIT NOT NULL DEFAULT 1,
	FOREIGN KEY (user_id) REFERENCES [USER](user_id),
);

-- Thêm dữ liệu mẫu cho bảng Campus
INSERT INTO CAMPUS (campus_code, campus_name, location)
VALUES 
('FPT-HN', 'FPT Hà Nội', 'Hà Nội'),
('FPT-HCM', 'FPT Hồ Chí Minh', 'Hồ Chí Minh'),
('FPT-DN', 'FPT Đà Nẵng', 'Đà Nẵng');

-- Thêm dữ liệu mẫu cho bảng Role
INSERT INTO ROLE (name, description)
VALUES 
('admin', 'Administrator with full access'),
('manager', 'Manager with project management privileges'),
('student', 'Student participating in projects'),
('mentor', 'Mentor providing guidance to projects'),
('sponsor', 'Sponsor investing in projects');

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
INSERT INTO [USER] (full_name, [password], email, phone, campus_id, role_id)
VALUES 
('Admin User', 'hashed_password', 'admin@uniexetask.com', '0901000001', 1, 1),
('Manager User', 'hashed_password', 'manager@uniexetask.com', '0901000002', 2, 2),
('Student User', 'hashed_password', 'student@uniexetask.com', '0901000003', 3, 3),
('Mentor User', 'hashed_password', 'mentor@uniexetask.com', '0901000004', 1, 4),
('Sponsor User', 'hashed_password', 'sponsor@uniexetask.com', '0901000005', 2, 5);

-- Thêm dữ liệu mẫu cho bảng ROLE_PERMISSION
INSERT INTO ROLE_PERMISSION (role_id, permission_id)
VALUES 
(1, 1), (1, 2), (1, 3), -- Admin có toàn quyền
(2, 1), (2, 2),         -- Manager có quyền xem và chỉnh sửa dự án
(3, 1),                 -- Student chỉ có quyền xem dự án
(4, 1),                 -- Mentor có quyền xem dự án
(5, 1);                 -- Sponsor có quyền xem dự án

-- Thêm dữ liệu mẫu cho bảng CHAT_GROUP
INSERT INTO CHAT_GROUP (chatbox_name, created_by, owner_id, type)
VALUES 
('Admin Group', 1, 1, 'Public'),
('Project Chat', 2, 2, 'Private');

-- Thêm dữ liệu mẫu cho bảng CHAT_MESSAGE
INSERT INTO CHAT_MESSAGE (chat_group_id, user_id, message_content)
VALUES 
(1, 1, 'Welcome to the Admin Group!'),
(2, 2, 'Project discussions start here.');

-- Thêm dữ liệu mẫu cho bảng SUBJECT
INSERT INTO SUBJECT (subject_code, subject_name, description, status)
VALUES 
('EXE101', 'Entrepreneurship Basics', 1, 'Status 1'),
('EXE102', 'Advanced Entrepreneurship', 2, 'Status 2');

-- Thêm dữ liệu mẫu cho bảng PROJECT
INSERT INTO PROJECT (topic_code, topic_name, description, start_date, end_date, subject_id, score, status)
VALUES 
('TP001', 'Green Energy', 'Research on renewable energy', '2024-09-01', '2025-01-01', 1, 90, 'Status 1'),
('TP002', 'Smart City', 'Building smart city systems', '2024-09-01', '2025-01-01', 2, 85, 'Status 2');

-- Thêm dữ liệu mẫu cho bảng TASK
INSERT INTO TASK (project_id, name, description, due_date, status)
VALUES 
(1, 'Research Phase', 'Complete the research phase of the project', '2024-11-01', 'Status 1'),
(2, 'Prototype', 'Build a prototype for the smart city project', '2024-12-01', 'Status 2');

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

-- Thêm dữ liệu mẫu cho bảng REQUIREMENT
INSERT INTO REQUIREMENT (project_id, requestor_id, content, status)
VALUES 
(1, 1, 'Request for additional resources on renewable energy', 'Status 1'),
(2, 2, 'Request for smart city infrastructure guidance', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng RESOURCE
INSERT INTO RESOURCE (project_id, name, type, url, upload_by)
VALUES 
(1, 'Energy Research Report', 'Status 1', 'http://example.com/energy_report.pdf', 1),
(2, 'Smart City Plan', 'Status 2', 'http://example.com/smart_city_plan.pdf', 2);

-- Thêm dữ liệu mẫu cho bảng SPONSOR
INSERT INTO SPONSOR (user_id, type, investment_field, status)
VALUES 
(5, 'Status 1', 'Renewable Energy', 'Status 1'),
(5, 'Status 2', 'Smart Infrastructure', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng PROJECT_SPONSOR
INSERT INTO PROJECT_SPONSOR (project_id, sponsor_id)
VALUES 
(1, 1), 
(2, 2);

-- Thêm dữ liệu mẫu cho bảng MENTOR
INSERT INTO MENTOR (user_id, title, specialty, status)
VALUES 
(4, 'Professor of Engineering', 'Renewable Energy', 'Status 1'),
(4, 'Expert in Smart Cities', 'Urban Planning', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng PROJECT_MENTOR
INSERT INTO PROJECT_MENTOR (project_id, mentor_id)
VALUES 
(1, 1), 
(2, 2);

-- Thêm dữ liệu mẫu cho bảng GROUP
INSERT INTO [GROUP] (project_id, group_name, status)
VALUES 
(1, 'Green Energy Team', 'Status 1'),
(2, 'Smart City Team', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng MEETING_SCHEDULE
INSERT INTO MEETING_SCHEDULE (group_id, mentor_id, location, meeting_date, duration, type, content, status)
VALUES 
(1, 1, 1, '2024-10-01', 60, 'Offline', 'Discussing project progress', 'Status 1'),
(2, 2, 2, '2024-11-01', 90, 'Online', 'Planning prototype development', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng STUDENT
INSERT INTO STUDENT (user_id, student_code, major, status)
VALUES 
(3, 'ST12345', 'Computer Science', 'Status 1'),
(3, 'ST67890', 'Information Technology', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng GROUP_MEMBER
INSERT INTO GROUP_MEMBER (group_id, student_id, role)
VALUES 
(1, 1, 'Leader'), 
(2, 2, 'Member');

-- Thêm dữ liệu mẫu cho bảng NOFITICATION
INSERT INTO NOFITICATION (sender_id, receiver_id, message, type, created_at, status)
VALUES 
(1, 2, 'Project update required', 'Offline', GETDATE(), 'Status 1'),
(2, 3, 'Meeting reminder', 'Online', GETDATE(), 'Status 2');

-- Thêm dữ liệu mẫu cho bảng GROUP_INVITE
INSERT INTO GROUP_INVITE (group_id, notification_id, inviter_id, invitee_id, created_at, updated_at, status)
VALUES 
(1, 1, 1, 2, GETDATE(), GETDATE(), 'Status 1'),
(2, 2, 2, 3, GETDATE(), GETDATE(), 'Status 2');

-- Thêm dữ liệu mẫu cho bảng EVENT
INSERT INTO EVENT (name, description, start_date, end_date, location, reg_url, status)
VALUES 
('Innovation Workshop', 'Workshop on innovation and entrepreneurship', '2024-10-10', '2024-10-12', 'FPT Hà Nội', 'http://example.com/register', 'Status 1'),
('Tech Expo', 'Exhibition on smart city technologies', '2024-12-01', '2024-12-03', 'FPT Hồ Chí Minh', 'http://example.com/register', 'Status 2');

-- Thêm dữ liệu mẫu cho bảng REFRESHTOKEN
INSERT INTO REFRESH_TOKEN (user_id, token, expires, created, revoked, status)
VALUES 
(1, 'sample_token_123', '2025-01-01', GETDATE(), '2025-01-01', 1),
(2, 'sample_token_456', '2025-01-01', GETDATE(), '2025-01-01', 1);

