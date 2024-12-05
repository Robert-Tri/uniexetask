using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uniexetask.core.Models.Enums;
using uniexetask.services.Interfaces;

namespace uniexetask.services.BackgroundServices
{
    public class GroupInviteBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public GroupInviteBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndExpirePendingInvitesAsync();
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckAndExpirePendingInvitesAsync()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var groupInviteService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var pendingInvites = await groupInviteService.GetPendingInvitesAsync();

                var now = DateTime.Now;

                foreach (var invite in pendingInvites)
                {
                    if ((now - invite.CreatedDate).TotalDays > 1)
                    {
                        invite.Status = nameof(GroupInviteStatus.Expired);
                        groupInviteService.UpdateGroupInviteAsync(invite);
                    }
                }
            }
        }
    }
}
