using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using safespace.Data;
using safespace.DTOs;
using safespace.Model;


namespace safespace.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        //Dependence
        private readonly AppDbContext _context;

        //constructor
        public ChatController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost("StartChat")]
        public async Task<IActionResult> StartChat([FromBody] ChatDTO request)
        {
            var UserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (UserIdClaim == null)
                return Unauthorized();

            var UserId = int.Parse(UserIdClaim.Value);

            var chat = await _context.Chat
                .Include(c => c.Messages)
                .Include(c => c.Doctor)
                .FirstOrDefaultAsync(c => c.DoctorId == request.DoctorId && c.PatientProfileId == UserId);

            if (chat == null)
            {
                chat = new Chats
                {
                    DoctorId = request.DoctorId,
                    PatientProfileId = UserId,
                    LastMessage = "",
                    LastMessageTime = Date.GetEgyptTime(),
                    //Messages = new List<Message>()
                };
                _context.Chat.Add(chat);
                await _context.SaveChangesAsync();
            }

            chat = await _context.Chat
              .Include(c => c.Doctor)
              .FirstOrDefaultAsync(c => c.id == chat.id);

            return Ok(new
            {
                ChatId = chat.id,
                DoctorId = chat.DoctorId,
                //DoctorName = chat.Doctor.FullName,
                //DoctorImage = chat.Doctor.ProfileImageUrl,
                UserId = chat.PatientProfileId,
                //Messages = messages,
                LastMessage = chat.LastMessage,
                LastMessageTime = chat.LastMessageTime
            });
        }


        [HttpGet("MyChats/{UserId}")]
        public async Task<IActionResult> GetMyChats(int UserId)
        {
            var cutoff = Date.GetEgyptTime().AddHours(-24);
            var chats = await _context.Chat
                .Where(c => c.PatientProfileId == UserId)
                .OrderByDescending(c => c.LastMessageTime)
                .Select(c => new {
                    c.id,
                    c.DoctorId,
                    DoctorName = c.Doctor.FullName,
                    LastMessage=c.LastMessageTime >= cutoff ? c.LastMessage : "",
                    Time = c.LastMessageTime >= cutoff ? c.LastMessageTime.ToString("hh:mm tt") : ""
                }).ToListAsync();
            var result = chats.Select(c =>
            {
                var name = c.DoctorName ?? "";

                var cleaned = name
                    .Replace("Dr.", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Ms.", "", StringComparison.OrdinalIgnoreCase)
                    .Replace("Mr.", "", StringComparison.OrdinalIgnoreCase)
                    .Trim();

                return new
                {
                    c.id,
                    c.DoctorId,
                    c.DoctorName,

                    DoctorInitial = string.IsNullOrWhiteSpace(cleaned)
                        ? ""
                        : char.ToUpper(cleaned[0]).ToString(),

                    c.LastMessage,
                    c.Time
                };
            });

            return Ok(result);
        }
        [HttpGet("Messages/{chatId}")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            var cutoff = Date.GetEgyptTime().AddHours(-24);

            var messages = await _context.Message
                .Where(m =>m.chatId == chatId &&
                    (m.sendAt >= cutoff || m.IsSaved==true))
                .OrderBy(m => m.sendAt)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost("SaveMessage/{messageId}")]
        public async Task<IActionResult> SaveMessage(int messageId)
        {
            var message = await _context.Message.FindAsync(messageId);

            if (message == null)
                return NotFound();

            message.IsSaved = !message.IsSaved;

            await _context.SaveChangesAsync();

            return Ok(new {
                message=message.IsSaved? "Saved":"unsaved",
                isSaved=message.IsSaved
                });
        }
    }
}
