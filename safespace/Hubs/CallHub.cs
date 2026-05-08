using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace safespace.Hubs
{
    [Authorize]
    public class CallHub : Hub
    {
        public async Task JoinCallRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Call_{roomId}");

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.Group($"Call_{roomId}")
                .SendAsync("ParticipantJoined", new
                {
                    connectionId = Context.ConnectionId,
                    userId = userId
                });
        }
        /*
        public async Task LeaveCallRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Call_{roomId}");

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.Group($"Call_{roomId}")
                .SendAsync("ParticipantLeft", new
                {
                    connectionId = Context.ConnectionId,
                    userId = userId
                });
        }

        public async Task SendOffer(string roomId, string targetConnectionId, string sdp)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.Client(targetConnectionId)
                .SendAsync("ReceiveOffer", new
                {
                    fromConnectionId = Context.ConnectionId,
                    fromUserId = userId,
                    sdp = sdp
                });
        }

        public async Task SendAnswer(string targetConnectionId, string sdp)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.Client(targetConnectionId)
                .SendAsync("ReceiveAnswer", new
                {
                    fromConnectionId = Context.ConnectionId,
                    fromUserId = userId,
                    sdp = sdp
                });
        }

        public async Task SendIceCandidate(string targetConnectionId, string candidate, string sdpMid, int sdpMLineIndex)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.Client(targetConnectionId)
                .SendAsync("ReceiveIceCandidate", new
                {
                    fromConnectionId = Context.ConnectionId,
                    fromUserId = userId,
                    candidate = candidate,
                    sdpMid = sdpMid,
                    sdpMLineIndex = sdpMLineIndex
                });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }*/
        public async Task SendChatMessage(string roomId, string message)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "User"; // لو عندك اسم المستخدم في التوكن

            await Clients.Group($"Call_{roomId}")
                .SendAsync("ReceiveMessage", new
                {
                    fromUserId = userId,
                    fromUserName = userName,
                    message = message,
                    timestamp = DateTime.UtcNow
                });
        }

        // 3. الخروج من الغرفة
        public async Task LeaveCallRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Call_{roomId}");

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.Group($"Call_{roomId}")
                .SendAsync("ParticipantLeft", new
                {
                    connectionId = Context.ConnectionId,
                    userId = userId
                });
        }

        // --- ميثودز الـ WebRTC Signaling (عشان نقل الصوت) ---

        public async Task SendOffer(string roomId, string targetConnectionId, string sdp)
        {
            await Clients.Client(targetConnectionId)
                .SendAsync("ReceiveOffer", new
                {
                    fromConnectionId = Context.ConnectionId,
                    sdp = sdp
                });
        }

        public async Task SendAnswer(string targetConnectionId, string sdp)
        {
            await Clients.Client(targetConnectionId)
                .SendAsync("ReceiveAnswer", new
                {
                    fromConnectionId = Context.ConnectionId,
                    sdp = sdp
                });
        }

        public async Task SendIceCandidate(string targetConnectionId, string candidate, string sdpMid, int sdpMLineIndex)
        {
            await Clients.Client(targetConnectionId)
                .SendAsync("ReceiveIceCandidate", new
                {
                    fromConnectionId = Context.ConnectionId,
                    candidate = candidate,
                    sdpMid = sdpMid,
                    sdpMLineIndex = sdpMLineIndex
                });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // ممكن هنا تضيفي Logic لو حد قفل المتصفح فجأة يتبعت تنبيه "Left"
            await base.OnDisconnectedAsync(exception);
        }
    }
}