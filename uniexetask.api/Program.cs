using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using uniexetask.api.BackgroundServices;
using uniexetask.services.Hubs;
using uniexetask.api.Middleware;
using uniexetask.infrastructure.ServiceExtension;
using uniexetask.services;
using uniexetask.services.BackgroundServices;
using uniexetask.services.Interfaces;
using Unitask.shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("https://localhost:3000", "https://uniexetask.netlify.app")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddSingleton(
    StorageClient.Create(
        GoogleCredential.FromFile(
            Path.Combine(Directory.GetCurrentDirectory(), "exeunitask-firebase-adminsdk-3jz7t-66373e3f35.json")
        )
    )
);

builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddDIServices(builder.Configuration);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<ICampusService, CampusService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IMentorService, MentorService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITopicService, TopicService>();
builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IChatGroupService, ChatGroupService>();
builder.Services.AddScoped<IWorkShopService, WorkShopService>();
builder.Services.AddScoped<ITimeLineService, TimeLineService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskAssignService, TaskAssignService>();
builder.Services.AddScoped<IReqMemberService, ReqMemberService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IProjectProgressService, ProjectProgressService>();
builder.Services.AddScoped<ITaskProgressService, TaskProgressService>();
builder.Services.AddScoped<IMemberScoreService, MemberScoreService>();
builder.Services.AddScoped<IMilestoneService, MilestoneService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IProjectScoreService, ProjectScoreService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ITaskDetailService, TaskDetailService>();
builder.Services.AddScoped<IReqTopicService, ReqTopicService>();
builder.Services.AddScoped<IMeetingScheduleService, MeetingScheduleService>();
builder.Services.AddScoped<IConfigSystemService, ConfigSystemService>();
builder.Services.AddScoped<ITopicForMentorService, TopicForMentorService>();

builder.Services.AddHostedService<TimelineBackgroundService>();
builder.Services.AddHostedService<TaskBackgroundService>();
builder.Services.AddHostedService<GroupInviteBackgroundService>();



builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddAuthorization(options =>
{
    //User
    options.AddPolicy("CanViewUser", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "view_user")));

    options.AddPolicy("CanCreateUser", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "create_user")));

    options.AddPolicy("CanEditUser", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "edit_user")));

    options.AddPolicy("CanDeleteUser", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "delete_user")));
    options.AddPolicy("CanImportUser", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "import_user")));

    //Meeting Schedule
    options.AddPolicy("CanViewMeetingSchedule", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "view_meeting_schedule")));

    options.AddPolicy("CanCreateMeetingSchedule", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "create_meeting_schedule")));

    options.AddPolicy("CanEditMeetingSchedule", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "edit_meeting_schedule")));

    options.AddPolicy("CanDeleteMeetingSchedule", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "delete_meeting_schedule")));

    // Document
    options.AddPolicy("CanViewDocument", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "view_document")));
    options.AddPolicy("CanUploadDocument", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "upload_document")));

    options.AddPolicy("CanEditDocument", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "edit_document")));

    options.AddPolicy("CanDeleteDocument", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "delete_document")));

    options.AddPolicy("CanDownloadDocument", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "download_document")));

    // TimeLine
    options.AddPolicy("CanViewTimeline", policy =>
    policy.RequireAssertion(context =>
        context.User.HasClaim(c => c.Type == "permissions" && c.Value == "view_timeline")));

    options.AddPolicy("CanEditTimeline", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "edit_timeline")));

    // Workshop
    options.AddPolicy("CanViewWorkshop", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "view_workshop")));

    options.AddPolicy("CanCreateWorkshop", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "create_workshop")));

    options.AddPolicy("CanEditWorkshop", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "edit_workshop")));

    options.AddPolicy("CanDeleteWorkshop", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permissions" && c.Value == "delete_workshop")));

    //Group
    //Project
});
builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
/*builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});*/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowSpecificOrigins"); // Thêm dòng này
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<PermissionsMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<UserHub>("/userHub");
app.Run();