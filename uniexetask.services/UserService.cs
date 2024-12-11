using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Hubs;
using uniexetask.services.Interfaces;
using uniexetask.shared.Extensions;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;

namespace uniexetask.services
{
    public class UserService : IUserService
    {
        public IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<UserHub> _hubContext;
        private readonly IEmailService _emailService;
        private readonly IRoleService _roleService;
        private readonly ICampusService _campusService;
        private readonly ISubjectService _subjectService;
        private readonly IMentorService _mentorService;

        public UserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IHubContext<UserHub> hubContext, 
            IEmailService emailService, 
            IRoleService roleService, 
            ISubjectService subjectService, 
            ICampusService campusService,
            IMentorService mentorService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
            _emailService = emailService;
            _roleService = roleService;
            _subjectService = subjectService;
            _campusService = campusService;
            _mentorService = mentorService;
        }

        public async Task<bool> CheckDuplicateUser(string email, string phone)
        {
            var users = (await _unitOfWork.Users.GetAsync(filter: u => (u.Email == email || u.Phone == phone) && u.IsDeleted == false));
            if (users.Any())
            {
                return true;
            }
            return false;
        } 
        
        public async Task<bool> CheckDuplicateUserForUpdate(int userId, string email, string phone)
        {
            var users = await _unitOfWork.Users.GetAsync(filter: u => u.UserId != userId);
            var existedUser = users.Where(u => (u.Email == email || u.Phone == phone) && u.IsDeleted == false);
            if (existedUser.Any())
                return true;
            return false;
        }

        public async Task<User> CreateUser(User user)
        {
            var existedUser = await CheckDuplicateUser(user.Email, user.Phone);
            if (user != null)
            {
                await _unitOfWork.Users.InsertAsync(user);

                var result = _unitOfWork.Save();

                if (result > 0)
                    return user;
            }
            throw new Exception("Failed to create user.");
        }

        public async Task<User> CreateUserExcel(User user)
        {
            var existedUser = await CheckDuplicateUser(user.Email, user.Phone);
            if (existedUser)
            {
                throw new Exception("Email or phone number already exists.");
            }

            if (user != null)
            {
                await _unitOfWork.Users.InsertAsync(user);

                var result = _unitOfWork.Save();

                if (result > 0)
                {
                    return user;
                }
            }

            throw new Exception("Failed to create user.");
        }


        public async Task<bool> DeleteUser(int userId)
        {

            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDAsync(userId);
                if (users != null)
                {
                    users.IsDeleted = true; 
                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            //var usersList = await _unitOfWork.Users.GetAsync(includeProperties: "Campus,Role");
            var usersList = await _unitOfWork.Users.GetUsersWithCampusAndRole();
            return usersList;
        }

        public async Task<User?> GetUserById(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDAsync(userId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<IEnumerable<User>> SearchUsersByEmailAsync(string query)
        {
            return await _unitOfWork.Users.SearchUsersByEmailAsync(query);
        }
        
        public async Task<User?> GetUserByIdWithCampusAndRoleAndStudents(int userId)
        {
            if (userId > 0)
            {
                var users = await _unitOfWork.Users.GetByIDWithCampusAndRoleAndStudents(userId);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<bool> UpdateUser(User user)
        {

            if (user != null)
            {
                var updatedUser = await _unitOfWork.Users.GetByIDAsync(user.UserId);
                if (updatedUser != null)
                {
                    updatedUser.FullName = user.FullName;
                    updatedUser.Email = user.Email;
                    updatedUser.Phone = user.Phone;
                    if(user.CampusId != 0)
                        updatedUser.CampusId = user.CampusId;

                    _unitOfWork.Users.Update(updatedUser);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    return false;
                }
            }
            return false;
        }

        public async Task<IEnumerable<User>> SearchStudentByStudentCodeAsync(string query)
        {
            return await _unitOfWork.Users.SearchStudentsByStudentCodeAsync(query);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            if (email != null)
            {
                var users = await _unitOfWork.Users.GetUserByEmailAsync(email);
                if (users != null)
                {
                    return users;
                }
            }
            return null;
        }

        public async Task<bool> ImportStudentFromExcel(int userId, IFormFile excelFile)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var excelList = new List<ExcelModel>();
                var emailList = new HashSet<string>();
                var phoneList = new HashSet<string>();
                var studentCodeList = new HashSet<string>();

                var campusesList = await _campusService.GetAllCampus();
                var subjectList = await _subjectService.GetSubjects();
                var rolesList = await _roleService.GetAllRole();

                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var campusCode = worksheet.Cells[row, 6].Text;
                            var roleName = worksheet.Cells[row, 7].Text;
                            var khoiText = worksheet.Cells[row, 8].Text;
                            var subjectCode = worksheet.Cells[row, 9].Text;
                            string studentCode = worksheet.Cells[row, 10].Text;
                            string major = worksheet.Cells[row, 11].Text;
                            string lecturer = worksheet.Cells[row, 12].Text;

                            var lecturerUser = await _mentorService.GetMentorByEmail(lecturer);

                            int campusId = campusesList
                            .Where(c => c.CampusCode.Equals(campusCode, StringComparison.OrdinalIgnoreCase))
                            .Select(c => c.CampusId)
                            .FirstOrDefault();


                            int subjectId = subjectList
                            .Where(s => s.SubjectCode.Equals(subjectCode, StringComparison.OrdinalIgnoreCase))
                            .Select(s => s.SubjectId)
                            .FirstOrDefault();


                            int roleId = rolesList
                            .Where(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                            .Select(r => r.RoleId)
                            .FirstOrDefault();


                            string fullname = worksheet.Cells[row, 2].Text;
                            string password = worksheet.Cells[row, 3].Text;
                            string emailUser = worksheet.Cells[row, 4].Text;
                            string phoneUser = worksheet.Cells[row, 5].Text;
                            int khoiNumber = int.Parse(khoiText.Substring(1));

                            if (emailList.Contains(emailUser))
                            {
                                throw new Exception($"Duplicate email found: {emailUser}");
                            }
                            emailList.Add(emailUser);

                            if (studentCodeList.Contains(studentCode))
                            {
                                throw new Exception($"Duplicate student code found: {studentCode}");
                            }
                            studentCodeList.Add(studentCode);

                            if (!emailUser.Contains("@"))
                            {
                                throw new Exception($"Invalid email format: {emailUser}. Email must contain '@'.");
                            }

                            if (phoneList.Contains(phoneUser))
                            {
                                throw new Exception($"Duplicate phone number found: {phoneUser}");
                            }
                            phoneList.Add(phoneUser);

                            if (!long.TryParse(phoneUser, out _))
                            {
                                throw new Exception($"Invalid phone number (should be numeric): {phoneUser}");
                            }

                            if (khoiNumber >= 19)
                            {
                                password = PasswordHasher.GenerateRandomPassword(8);
                            }

                            var excel = new ExcelModel
                            {
                                FullName = fullname,
                                Password = password,
                                Email = emailUser,
                                Phone = phoneUser,
                                CampusId = campusId,
                                IsDeleted = false,
                                RoleId = roleId,
                                LecturerId = lecturerUser.MentorId,
                                StudentCode = studentCode,
                                Major = major,
                                SubjectId = subjectId,
                                IsCurrentPeriod = true
                            };

                            excelList.Add(excel);
                        }
                    }
                }

                var emailTasks = new List<System.Threading.Tasks.Task>();
                int count = 1;
                foreach (var excelModel in excelList)
                {
                    string rawPassword = excelModel.Password;

                    excelModel.Password = PasswordHasher.HashPassword(excelModel.Password);

                    var userModel = new UserModel
                    {
                        FullName = excelModel.FullName,
                        Password = excelModel.Password,
                        Email = excelModel.Email,
                        Phone = excelModel.Phone,
                        CampusId = excelModel.CampusId,
                        IsDeleted = false,
                        RoleId = excelModel.RoleId,
                        Avatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg"
                    };

                    var userEntity = _mapper.Map<User>(userModel);

                    var existedUser = await CheckDuplicateUser(userEntity.Email, userEntity.Phone);
                    if (existedUser)
                    {
                        throw new Exception("Email or phone number already exists.");
                    }
                    await _unitOfWork.Users.InsertAsync(userEntity);
                    _unitOfWork.Save();
                    var studentModel = new StudentModel
                    {
                        UserId = userEntity.UserId,
                        LecturerId = excelModel.LecturerId,
                        StudentCode = excelModel.StudentCode,
                        Major = excelModel.Major,
                        SubjectId = excelModel.SubjectId,
                        IsCurrentPeriod = true
                    };

                    var studentEntity = _mapper.Map<Student>(studentModel);

                    await _unitOfWork.Students.InsertAsync(studentEntity);

                    await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveImportUserProgress", count++, excelList.Count);

                    var userEmail = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <style>
                            body {{
                                font-family: Arial, sans-serif;
                                line-height: 1.6;
                                padding: 20px;
                                background-color: #ffffff;
                            }}
                            h2 {{
                                color: #333;
                            }}
                            p {{
                                margin: 0 0 10px;
                            }}
                        </style>
                    </head>
                    <body>
                        <h2>Dear {userModel.FullName},</h2>
                        <p>We are sending you the following login account information:</p>
                        <ul>
                            <li><strong>Account:</strong> {userModel.Email}</li>
                            <li><strong>Password: </strong><em>{rawPassword}</em>.</li>
                        </ul>
                        <p>We recommend that you change your password after logging in for the first time.</p>
                        <p>Looking forward to your participation.</p>
                        <p>This is an automated email, please do not reply to this email. If you need assistance, please contact us at unitask68@gmail.com.</p>
                    </body>
                    </html>
                    ";

                    var emailTask = _emailService.SendEmailAsync(userModel.Email, "Account UniEXETask", userEmail);
                    emailTasks.Add(emailTask);
                }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                System.Threading.Tasks.Task.WhenAll(emailTasks);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                return false;
            }
        }
    }
}
