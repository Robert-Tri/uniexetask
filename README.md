
---

## 📘 README cho Backend – UniEXETask

📁 Repo: [uniexetask](https://github.com/Robert-Tri/uniexetask)

```markdown
# 🧠 UniEXETask – Backend

**UniEXETask** là hệ thống quản lý dự án khởi nghiệp dành cho sinh viên Đại học FPT.
Backend được xây dựng bằng ASP.NET API để đảm bảo hiệu suất, bảo mật và khả năng mở rộng.

---

## 🎯 Mục tiêu dự án

- Hỗ trợ sinh viên kết nối, lập nhóm, chọn đề tài và giảng viên
- Giúp giảng viên giám sát tiến độ và đánh giá nhóm khởi nghiệp
- Tối ưu hóa quy trình học tập môn EXE – Khởi nghiệp Trải nghiệm

---

## 🛠️ Công nghệ sử dụng

- **Backend**: ASP.NET Web API (C#)
- **Database**: SQL Server
- **Authentication**: Firebase / Google OAuth

---

## 📂 Cấu trúc API

- `GET /projects` – Lấy danh sách đề tài
- `POST /groups` – Tạo nhóm mới
- `PUT /tasks/:id` – Cập nhật tiến độ
- `GET /users/:id` – Thông tin người dùng
- `POST /auth/google` – Đăng nhập bằng Google

---

## ⚙️ Cài đặt

```bash
git clone https://github.com/Robert-Tri/uniexetask.git
cd uniexetask
dotnet restore
dotnet run
