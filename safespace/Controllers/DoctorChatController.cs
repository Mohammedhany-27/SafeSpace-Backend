using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using safespace.Data;
using System.Security.Claims;

namespace safespace.Controllers
{
    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorChatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("MyRecentChats")]
        public async Task<IActionResult> MyRecentChats()
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            // بنحدد الـ 24 ساعة
            var cutoff = safespace.Model.Date.GetEgyptTime().AddHours(-24);

            // عشان نرجع مسار الصورة كامل 
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var chats = await _context.Chat
                .Where(c =>
                    c.DoctorId == doctorId &&
                    // التعديل هنا: هنجيب الشات لو آخر رسالة من أقل من 24 ساعة، "أو" لو الشات جواه أي رسالة محفوظة
                    (c.LastMessageTime >= cutoff || c.Messages.Any(m => m.IsSaved == true)) &&
                    c.Messages.Any(m => m.senderId == c.PatientProfileId)
                )
                .OrderByDescending(c => c.LastMessageTime)
                .Select(c => new
                {
                    ChatId = c.id,
                    PatientId = c.PatientProfileId,

                    // 1. التعديل هنا: بنستخدم الـ DisplayName من ملف الـ PatientProfile
                    PatientName = c.User.DisplayName,
                    PatientEmail = c.User.Email,

                    // 2. التعديل هنا: بنجيب صورة المريض لو موجودة
                    ImageUrl = string.IsNullOrEmpty(c.User.ProfileImageUrl)
                        ? null
                        : baseUrl + c.User.ProfileImageUrl,

                    LastMessage = c.LastMessage,
                    Time = c.LastMessageTime.ToString("hh:mm tt")
                })
                .ToListAsync();

            var result = chats.Select(c =>
            {
                var name = c.PatientName ?? "";

                return new
                {
                    c.ChatId,
                    c.PatientId,
                    c.PatientName,
                    c.PatientEmail,
                    c.ImageUrl, // رجعنا الصورة

                    // بنسيب الـ Initial احتياطي للفرونت لو المريض معندوش صورة
                    PatientInitial = string.IsNullOrWhiteSpace(name)
                        ? ""
                        : char.ToUpper(name.Trim()[0]).ToString(),
                    c.LastMessage,
                    c.Time
                };
            });

            return Ok(result);
        }

        [HttpGet("Messages/{chatId}")]
        public async Task<IActionResult> GetMessages(int chatId)
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            var chat = await _context.Chat
                .FirstOrDefaultAsync(c => c.id == chatId && c.DoctorId == doctorId);

            if (chat == null)
                return NotFound("Chat not found");

            var cutoff = safespace.Model.Date.GetEgyptTime().AddHours(-24);

            var messages = await _context.Message
                .Where(m =>
                    m.chatId == chatId &&
                    (m.sendAt >= cutoff || m.IsSaved == true)
                )
                .OrderBy(m => m.sendAt)
                .Select(m => new
                {
                    MessageId = m.id,
                    ChatId = m.chatId,
                    SenderId = m.senderId,
                    IsFromPatient = m.senderId == chat.PatientProfileId,
                    Text = m.MessageText,
                    Time = m.sendAt.ToString("hh:mm tt"),
                    m.isRead,
                    m.IsSaved
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}