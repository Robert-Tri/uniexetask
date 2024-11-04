using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using uniexetask.api.BackgroundServices;
using uniexetask.api.Extensions;
using uniexetask.api.Hubs;
using uniexetask.api.Middleware;
using uniexetask.infrastructure.ServiceExtension;
using uniexetask.services;
using uniexetask.services.BackgroundServices;
using uniexetask.services.Interfaces;
using Unitask.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", builder =>
    {
        builder.WithOrigins("https://localhost:3000", "https://localhost:7289", "https://visionary-melomakarona-558966.netlify.app")
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
builder.Services.AddScoped<IUsagePlanService, UsagePlanService>();
builder.Services.AddScoped<IMemberScoreService, MemberScoreService>();
builder.Services.AddScoped<IMilestoneService, MilestoneService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IProjectScoreService, ProjectScoreService>();

builder.Services.AddHostedService<TimelineBackgroundService>();
builder.Services.AddHostedService<TaskBackgroundService>();



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
app.Run();
