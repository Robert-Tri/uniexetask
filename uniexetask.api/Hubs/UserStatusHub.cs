using Microsoft.AspNetCore.SignalR;

namespace uniexetask.api.Hubs
{
    public class UserStatusHub : Hub
    {
        public async Task UpdateUserStatus(string userId, bool isOnline)
        {
            await Clients.All.SendAsync("ReceiveUserStatus", userId, isOnline);
        }
    }
}
