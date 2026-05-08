using Microsoft.AspNetCore.SignalR;
using safespace.Data;
using safespace.Model;
using safespace.DTOs;
namespace safespace.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;

        public ChatHub(AppDbContext context)
        {
            _context = context;
        }

        // Join chat room
        public async Task JoinChat(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Chat_{chatId}");
        }

        // Send message
        public async Task SendMessage(MessageDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MessageText))
                return;

            // بتأكد أن الشات موجودالاول
            var chat = await _context.Chat.FindAsync(dto.chatId);
            if (chat == null)
            {
                return;
            }

            // إنشاء الرسالة
            var message = new Message
            {
                chatId = dto.chatId,
                senderId = dto.senderId,
                MessageText = dto.MessageText,
                isRead = false,
                IsSaved = false,
                sendAt = Date.GetEgyptTime()
            };

            _context.Message.Add(message);

            chat.LastMessage = dto.MessageText;
            chat.LastMessageTime = Date.GetEgyptTime();

            await _context.SaveChangesAsync();

            //الإرسال للـ Clients
            await Clients.Group($"Chat_{dto.chatId}")
                .SendAsync("ReceiveMessage", new
                {
                    messageId = message.id,
                    chatId = dto.chatId,
                    senderId = dto.senderId,
                    text = message.MessageText,
                    time = message.sendAt.ToString("hh:mm tt"),
                    isRead = message.isRead
                });
        }
        // Typing
        public async Task Typing(int chatId, int userId)
        {
            await Clients.Group($"Chat_{chatId}")
                .SendAsync("UserTyping", new { chatId, userId });
        }

        // Read
        public async Task MarkAsRead(int messageId)
        {
            var message = await _context.Message.FindAsync(messageId);

            if (message == null) return;

            message.isRead = true;
            await _context.SaveChangesAsync();

            await Clients.Group($"Chat_{message.chatId}")
                .SendAsync("MessageRead", new
                {
                    messageId,
                    chatId = message.chatId
                });
        }
        /*
        //اونلاين
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("UserOnline", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        //اوفلاين
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("UserOffline", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }*/
    }
}