using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using uniexetask.api.Extensions;
using uniexetask.core.Interfaces;
using uniexetask.core.Models.Enums;

namespace uniexetask.services.BackgroundServices
{
    public class TaskBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TaskBackgroundService> _logger;

        public TaskBackgroundService(IServiceScopeFactory scopeFactrory, ILogger<TaskBackgroundService> logger)
        {
            _scopeFactory = scopeFactrory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                CheckTaskDeadline();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
        private async Task CheckTaskDeadline()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var tasks = await unitOfWork.Tasks.GetAsync(filter: t => t.Status == nameof(TasksStatus.Not_Started) || t.Status == nameof(TasksStatus.In_Progress) || t.Status == nameof(TasksStatus.Overdue));
                var emailTasks = new List<Task>();

                foreach (var task in tasks)
                {
                    if (task.Status == nameof(TasksStatus.Not_Started) && task.StartDate.Date == DateTime.Now.Date)
                    {
                        task.Status = nameof(TasksStatus.In_Progress);
                        unitOfWork.Tasks.Update(task);
                        emailTasks.Add(NotifyTaskAssignmentAsync(task, emailService, "Your task has started. Please proceed!", "green"));
                    }
                    else if (task.Status == nameof(TasksStatus.In_Progress))
                    {
                        if (task.EndDate.Date == DateTime.Now.Date.AddDays(1))
                        {
                            emailTasks.Add(NotifyTaskAssignmentAsync(task, emailService, "The deadline for this task is approaching. Please complete any remaining work.", "orange"));
                        }
                        else if (task.EndDate < DateTime.Now.Date)
                        {
                            task.Status = nameof(TasksStatus.Overdue);
                            unitOfWork.Tasks.Update(task);
                            emailTasks.Add(NotifyTaskAssignmentAsync(task, emailService, "This task is now overdue. Please take immediate action!", "red"));
                        }
                    }
                }
                try
                {
                    await Task.WhenAll(emailTasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending email notifications.");
                }
                await unitOfWork.SaveAsync();

            }
        }

        private async Task NotifyTaskAssignmentAsync(core.Models.Task task, IEmailService emailService, string message, string messageColor)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var taskAssigns = await unitOfWork.TaskAssigns.GetAsync(filter: ts => ts.TaskId == task.TaskId);
                var emailTasks = new List<System.Threading.Tasks.Task>();

                foreach (var taskAssign in taskAssigns)
                {
                    var student = await unitOfWork.Students.GetByIDAsync(taskAssign.StudentId);
                    if (student == null) continue;

                    var user = await unitOfWork.Users.GetByIDAsync(student.UserId);
                    var groupMember = await unitOfWork.GroupMembers.GetAsync(filter: r => r.StudentId == student.StudentId);
                    var role = groupMember.FirstOrDefault()?.Role;

                    string emailContent = $@"
<p>Dear {user.FullName},</p>
<p>We would like to inform you of an important update regarding your assigned task. Please find the key details below:</p>
<h3>Task Overview:</h3>
<ul>
    <li><b>Task Name:</b> {task.TaskName}</li>
    <li><b>Deadline:</b> {task.EndDate:dd/MM/yyyy}</li>
    <li><b>Status:</b> {task.Status}</li>
</ul>
<p style='color: {messageColor};'><strong>{message}</strong></p>
{(role == "Leader" ? "<p>As the team leader, please ensure that your team members are aware and on track to complete their tasks by the deadline.</p>" : "")}
<p>Thank you for your attention to this task.</p>
<p>Best regards,<br>This is an automated email, please do not reply to this email. If you need assistance, please contact us at unitask68@gmail.com.</p>
";

                    var emailTask = emailService.SendEmailAsync(user.Email, "Task Notification", emailContent);
                    emailTasks.Add(emailTask);
                }

                await Task.WhenAll(emailTasks);
            }
        }
    }
}
