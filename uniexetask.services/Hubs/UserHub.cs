using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace uniexetask.services.Hubs
{
    [Authorize]
    public class UserHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }
        public async Task SendImportUserProgress(string userId, int current, int total)
        {
            await Clients.User(userId).SendAsync("ReceiveImportUserProgress", current, total);
        }
    }
}
