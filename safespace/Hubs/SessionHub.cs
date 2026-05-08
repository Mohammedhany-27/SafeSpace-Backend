using Microsoft.AspNetCore.SignalR;

namespace SafeSpace.Hubs
{
    public class SessionHub : Hub
    {
        public async Task JoinDoctorPage(int doctorId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Doctor_{doctorId}");
        }
    }
}