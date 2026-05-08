using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using safespace.Data;
using safespace.DTOs.safespace.DTOs;
using safespace.Model;
using System.Security.Claims;

namespace safespace.Controllers
{
    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorSessionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorSessionsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("MySessions")]
        public async Task<IActionResult> MySessions()
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (doctorIdClaim == null) return Unauthorized();
            var doctorId = int.Parse(doctorIdClaim.Value);

            var now = safespace.Model.Date.GetEgyptTime();

            var sessions = await _context.Sessions
                .Where(s => s.DoctorId == doctorId)
                .Include(s => s.User)
                .Include(s => s.AvailableSlots)
                .OrderByDescending(s => s.AvailableSlots.date)
                .ToListAsync();

            // الجلسات السابقة (History)
            var past = sessions
                .Where(s => s.AvailableSlots.date.Date < now.Date)
                .Select(s => new
                {
                    sessionId = s.id, 
                    userCode = s.User is PatientProfile p ? p.usercode : "usr-00000", 
                    date = s.AvailableSlots.date.ToString("yyyy-MM-dd"),
                    time = s.AvailableSlots.time,
                    type = s.AvailableSlots.Type.ToString(),
                    status = s.Status
                })
                .ToList();

            return Ok(new { past = past });
        }

        [HttpGet("{sessionId}")]
        public async Task<IActionResult> GetSessionDetails(int sessionId) 
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (doctorIdClaim == null) return Unauthorized();
            var doctorId = int.Parse(doctorIdClaim.Value);

            var session = await _context.Sessions
                .Include(s => s.User)
                .Include(s => s.AvailableSlots)
                .FirstOrDefaultAsync(s => s.id == sessionId && s.DoctorId == doctorId);

            if (session == null) return NotFound("Session not found");

            return Ok(new
            {
                sessionId = session.id, 
                userCode = session.User is PatientProfile p ? p.usercode : "usr-00000", 
                date = session.AvailableSlots.date.ToString("yyyy-MM-dd"),
                time = session.AvailableSlots.time,
                type = session.AvailableSlots.Type.ToString(),
                status = session.Status,
                notes = session.Notes ?? ""
            });
        }


        [HttpGet("{sessionId}/Notes")]
        public async Task<IActionResult> GetSessionNotes(int sessionId)
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // بندور على الجلسة ونتأكد إنها تخص الدكتور ده
            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.id == sessionId && s.DoctorId == doctorId);

            if (session == null)
                return NotFound("Session not found");

            // بنرجع النوتس بس، ولو فاضية نرجع نص فاضي
            return Ok(new { Notes = session.Notes ?? "" });
        }

      
        [HttpPost("{sessionId}/Notes")]
        public async Task<IActionResult> SaveSessionNotes(int sessionId, [FromBody] SessionNoteDto dto)
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.id == sessionId && s.DoctorId == doctorId);

            if (session == null)
                return NotFound("Session not found");

            // بنحدث النوتس ونحفظ
            session.Notes = dto.Notes;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Notes saved successfully" });
        }
    }
}