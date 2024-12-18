using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Transactions;
using uniexetask.core.Interfaces;
using uniexetask.core.Models;
using uniexetask.services.Hubs;
using uniexetask.services.Interfaces;
using uniexetask.shared.Extensions;
using uniexetask.shared.Models.Request;
using uniexetask.shared.Models.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Task = System.Threading.Tasks.Task;

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
        private readonly IStudentService _studentService;

        public UserService(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IHubContext<UserHub> hubContext, 
            IEmailService emailService, 
            IRoleService roleService, 
            ISubjectService subjectService, 
            ICampusService campusService,
            IMentorService mentorService,
            IStudentService studentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
            _emailService = emailService;
            _roleService = roleService;
            _subjectService = subjectService;
            _campusService = campusService;
            _mentorService = mentorService;
            _studentService = studentService;
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

        public async Task<IEnumerable<string>?> ImportStudentFromExcel(int userId, IFormFile excelFile)
        {
            try
            {
                _unitOfWork.BeginTransaction();
                var validationContext = await PrepareValidationContext();

                var (excelList, validationErrors) = await ValidateExcelData(excelFile, validationContext);

                if (validationErrors.Any())
                {
                    return validationErrors;
                }

                await ProcessImportedStudents(userId, excelList);

                _unitOfWork.Commit();

                return null;
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private async Task<ValidationImportUserContext> PrepareValidationContext()
        {
            return new ValidationImportUserContext
            {
                Campuses = await _campusService.GetAllCampus(),
                Subjects = await _subjectService.GetSubjects(),
                Roles = await _roleService.GetAllRole(),
                ExistingUsers = await _unitOfWork.Users.GetAllUsers(),
                ExistingStudents = await _studentService.GetAllStudent()
            };
        }

        private async Task<(List<ExcelModel> ExcelList, List<string> Errors)> ValidateExcelData(
            IFormFile excelFile,
            ValidationImportUserContext context)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            var errors = new List<string>();
            var excelList = new List<ExcelModel>();

            var emailList = new HashSet<string>();
            var phoneList = new HashSet<string>();
            var studentCodeList = new HashSet<string>();

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
                        string fullname = worksheet.Cells[row, 2].Text;
                        string? password = null;
                        User? AccountExists = new ();
                        string emailUser = worksheet.Cells[row, 3].Text;
                        string phoneUser = worksheet.Cells[row, 4].Text;
                        string campusCode = worksheet.Cells[row, 5].Text;
                        string roleName = worksheet.Cells[row, 6].Text;
                        string khoiText = worksheet.Cells[row, 7].Text;
                        string subjectCode = worksheet.Cells[row, 8].Text;
                        string studentCode = worksheet.Cells[row, 9].Text;
                        string major = worksheet.Cells[row, 10].Text;
                        string lecturer = worksheet.Cells[row, 11].Text;

                        //Check campus code
                        int campusId = context.Campuses
                        .Where(c => c.CampusCode.Equals(campusCode, StringComparison.OrdinalIgnoreCase))
                        .Select(c => c.CampusId)
                        .FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(campusCode))
                        {
                            errors.Add($"Line {row} | Campus Code | {campusCode} | Campus Code cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            if (campusId == 0) errors.Add($"Line {row} | Campus Code | {campusCode} | Invalid campus code (Ex: FPT-HN, FPT-HCM)");
                        }

                        //Check subject code
                        int subjectId = context.Subjects
                        .Where(s => s.SubjectCode.Equals(subjectCode, StringComparison.OrdinalIgnoreCase))
                        .Select(s => s.SubjectId)
                        .FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(subjectCode))
                        {
                            errors.Add($"Line {row} | Subject Code | {subjectCode} | Subject Code cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            if (subjectId == 0) errors.Add($"Line {row} | Subject Code | {subjectCode} | Invalid subject code (Ex: EXE101, EXE201)");
                        }

                        //Check role name
                        int roleId = context.Roles
                        .Where(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                        .Select(r => r.RoleId)
                        .FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(roleName))
                        {
                            errors.Add($"Line {row} | Role | {roleName} | Role cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            if (roleId == 0) errors.Add($"Line {row} | Role | {roleName} | Invalid role (Ex: student)");
                        }

                        //Check class
                        if (string.IsNullOrWhiteSpace(khoiText))
                        {
                            errors.Add($"Line {row} | Class | {khoiText} | Class cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            if (khoiText.Length > 1 && int.TryParse(khoiText.Substring(1), out int khoiNumber))
                            {
                                if (khoiNumber >= 19)
                                {
                                    password = PasswordHasher.GenerateRandomPassword(8);
                                }
                                else if (khoiNumber < 19 && khoiNumber > 0)
                                {
                                    password = null;
                                }
                            }
                            else
                            {
                                errors.Add($"Line {row} | Class | {khoiText} | Invalid class (Ex: K18, K19)");
                            }
                        }

                        //Check email
                        var existingUser = context.ExistingUsers.FirstOrDefault(user => user.Email == emailUser);
                        if (string.IsNullOrWhiteSpace(emailUser))
                        {
                            errors.Add($"Line {row} | Email | {emailUser} | Email cannot be blank or contain only white-space characters");
                        }
                        else if (!Regex.IsMatch(emailUser, emailPattern)) errors.Add($"Line {row} | Email | {emailUser} | Invalid email format");
                        else if (emailList.Contains(emailUser)) errors.Add($"Line {row} | Email | {emailUser} | Duplicate email in file");
                        else if (existingUser != null) 
                        {
                            if (existingUser.IsDeleted)
                            {
                                AccountExists = existingUser;
                            }
                            else
                            {
                                errors.Add($"Line {row} | Email | {emailUser} | Duplicate email in database");
                            }
                        }

                        //Check student code
                        if (string.IsNullOrWhiteSpace(studentCode))
                        {
                            errors.Add($"Line {row} | Student Code | {studentCode} | Student Code cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            if (studentCodeList.Contains(studentCode))
                            {
                                errors.Add($"Line {row} | Student Code | {studentCode} | Duplicate student code in file");
                            }
                            if (context.ExistingStudents.Any(student => student.StudentCode == studentCode))
                            {
                                if (existingUser != null && existingUser.IsDeleted)
                                {
                                    //do nothing
                                }
                                else
                                {
                                    errors.Add($"Line {row} | Student Code | {studentCode} | Duplicate student code in database");
                                }
                            }
                        }

                        //Check number phone
                        if (string.IsNullOrWhiteSpace(studentCode))
                        {
                            errors.Add($"Line {row} | Phone | {phoneUser} | Phone cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            if (!long.TryParse(phoneUser, out _))
                            {
                                errors.Add($"Line {row} | Phone | {phoneUser} | Invalid phone number (should be numeric)");
                                if (phoneList.Contains(phoneUser))
                                {
                                    errors.Add($"Line {row} | Phone | {phoneUser} | Duplicate phone number in file");
                                }
                                if (context.ExistingUsers.Any(user => user.Phone == phoneUser))
                                {
                                    if (existingUser != null && existingUser.IsDeleted)
                                    {
                                        //do nothing
                                    }
                                    else
                                    {
                                        errors.Add($"Line {row} | Phone | {phoneUser} | Duplicate phone number in database");
                                    }
                                }
                            }
                        }

                        Mentor? lecturerUser = null;
                        if (string.IsNullOrWhiteSpace(lecturer))
                        {
                            errors.Add($"Line {row} | Leaturer | {lecturer} | Leaturer cannot be blank or contain only white-space characters");
                        }
                        else
                        {
                            lecturerUser = await _mentorService.GetMentorByEmail(lecturer);
                            if (lecturerUser == null) errors.Add($"Line {row} | Leaturer | {lecturer} | Leatuter not found");
                        }

                        if (errors.Any())
                            continue;

                        emailList.Add(emailUser);
                        studentCodeList.Add(studentCode);
                        phoneList.Add(phoneUser);

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
                            IsCurrentPeriod = true,
                            AccountExists = AccountExists
                        };
                        excelList.Add(excel);
                    }
                }
            }

            return (excelList, errors);
        }

        private async Task ProcessImportedStudents(int userId, List<ExcelModel> excelList)
        {
            var emailTasks = new List<System.Threading.Tasks.Task>();
            int count = 1;
            var userList = new List<User>();
            foreach (var excelModel in excelList)
            {
                string? rawPassword = excelModel.Password;
                var passwordLine = string.Empty;

                if (!string.IsNullOrWhiteSpace(rawPassword))
                {
                    excelModel.Password = PasswordHasher.HashPassword(rawPassword);
                    passwordLine = $"<li><strong>Password:</strong> <em>{rawPassword}</em>.</li>";
                }
                if (excelModel.AccountExists.UserId != 0)
                {
                    excelModel.AccountExists.FullName = excelModel.FullName;
                    excelModel.AccountExists.Password = excelModel.Password;
                    excelModel.AccountExists.Email = excelModel.Email;
                    excelModel.AccountExists.Phone = excelModel.Phone;
                    excelModel.AccountExists.CampusId = excelModel.CampusId;
                    excelModel.AccountExists.IsDeleted = false;
                    excelModel.AccountExists.RoleId = excelModel.RoleId;
                    excelModel.AccountExists.Avatar = "https://res.cloudinary.com/dan0stbfi/image/upload/v1722340236/xhy3r9wmc4zavds4nq0d.jpg";
                    _unitOfWork.Users.Update(excelModel.AccountExists);
                }
                else
                {
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
                            {rawPassword}
                        </ul>
                        <p>Looking forward to your participation.</p>
                        <p>This is an automated email, please do not reply to this email. If you need assistance, please contact us at unitask68@gmail.com.</p>
                    </body>
                    </html>
                    ";

                    var emailTask = _emailService.SendEmailAsync(userModel.Email, "Account UniEXETask", userEmail);
                    emailTasks.Add(emailTask);
                }

                await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveImportUserProgress", count++, excelList.Count);
            }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            System.Threading.Tasks.Task.WhenAll(emailTasks);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _unitOfWork.Save();
        }

        private async Task<IEnumerable<string>> GetAllEmais()
        {
            return await _unitOfWork.Users.GetAllEmails();
        }
    }

    public class ValidationImportUserContext
    {
        public required IEnumerable<Campus> Campuses { get; set; }
        public required IEnumerable<Subject> Subjects { get; set; }
        public required IEnumerable<Role> Roles { get; set; }
        public required IEnumerable<User> ExistingUsers { get; set; }
        public required IEnumerable<Student> ExistingStudents { get; set; }
    }
}
