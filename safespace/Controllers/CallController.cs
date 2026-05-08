using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using safespace.Data;
using safespace.DTOs;
using safespace.Model;
using System.Security.Claims;

namespace safespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CallController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CallController(AppDbContext context)
        {
            _context = context;
        }
        /*
        // 🟢 1) Create Call (one-to-one أو group)
        [HttpPost("create")]
        public async Task<IActionResult> CreateCall(CreateCallDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.id == dto.SessionId);

            if (session == null)
                return NotFound("Session not found");

            // لازم يكون جزء من السيشن
            if (session.UserId != userId && session.DoctorId != userId)
                return Unauthorized("You are not part of this session");

            var existingCall = await _context.CallSessions
                .FirstOrDefaultAsync(c => c.SessionId == dto.SessionId && c.Status != "Ended");

            if (existingCall != null)
                return BadRequest("Call already exists");

            var call = new CallSession
            {
                SessionId = dto.SessionId,
                RoomId = Guid.NewGuid().ToString(),
                IsGroupCall = dto.IsGroupCall,
                MaxParticipants = dto.IsGroupCall ? 10 : 2,
                Status = "Pending",
                IsStarted = false
            };

            _context.CallSessions.Add(call);
            await _context.SaveChangesAsync();

            // add doctor
            _context.CallParticipants.Add(new CallParticipant
            {
                CallSessionId = call.Id,
                UserId = session.DoctorId,
                IsDoctor = true
            });

            // add patient
            _context.CallParticipants.Add(new CallParticipant
            {
                CallSessionId = call.Id,
                UserId = session.UserId
            });

            await _context.SaveChangesAsync();

            return Ok(call);
        }

        // 🟢 2) Start Call (doctor only)
        [HttpPost("start/{callId}")]
        public async Task<IActionResult> StartCall(int callId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var call = await _context.CallSessions
                .Include(c => c.Session)
                .FirstOrDefaultAsync(c => c.Id == callId);

            if (call == null)
                return NotFound("Call not found");

            if (call.Session.DoctorId != userId)
                return Unauthorized("Only doctor can start");

            if (call.Status == "Ended")
                return BadRequest("Call already ended");

            call.IsStarted = true;
            call.Status = "Started";

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Call started",
                callId = call.Id,
                roomId = call.RoomId
            });
        }

        // 🟢 3) Join Call
        [HttpPost("join")]
        public async Task<IActionResult> JoinCall(JoinCallDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var call = await _context.CallSessions
    .Include(c => c.Session)
        .ThenInclude(s => s.AvailableSlots)
    .Include(c => c.CallParticipants)
    .FirstOrDefaultAsync(c => c.Id == dto.CallSessionId);

            if (call == null)
                return NotFound("Call not found");

            if (!call.IsStarted)
                return BadRequest("Call not started");

            if (call.Status == "Ended")
                return BadRequest("Call ended");

            var exists = call.CallParticipants.Any(p => p.UserId == userId);

            if (!exists)
            {
                // لو one-to-one
                if (!call.IsGroupCall)
                    return Unauthorized("Not allowed in one-to-one");

                // لو جروب
                if (call.CallParticipants.Count >= call.MaxParticipants)
                    return BadRequest("Call is full");

                _context.CallParticipants.Add(new CallParticipant
                {
                    CallSessionId = call.Id,
                    UserId = userId
                });

                await _context.SaveChangesAsync();
            }

            var now = DateTime.UtcNow;

            if (!TimeSpan.TryParse(call.Session.AvailableSlots.time, out var slotTime))
            {
                return BadRequest("Invalid time format");
            }

            var sessionDateTime = call.Session.AvailableSlots.date.Date.Add(slotTime);

            var allowedStart = sessionDateTime.AddMinutes(-10);
            var allowedEnd = sessionDateTime.AddMinutes(30);

            if (now < allowedStart || now > allowedEnd)
            {
                return BadRequest("You can only join at the scheduled time");
            }
            return Ok(new
            {
                roomId = call.RoomId,
                callId = call.Id,
                isGroup = call.IsGroupCall
            });
        }

        // 🟢 4) End Call
        [HttpPost("end/{callId}")]
        public async Task<IActionResult> EndCall(int callId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var call = await _context.CallSessions
                .Include(c => c.Session)
                .FirstOrDefaultAsync(c => c.Id == callId);

            if (call == null)
                return NotFound("Call not found");

            if (call.Session.DoctorId != userId)
                return Unauthorized("Only doctor can end");

            call.Status = "Ended";
            call.IsStarted = false;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Call ended"
            });
        }

        // 🟢 5) Get Call Info
        [HttpGet("{callId}")]
        public async Task<IActionResult> GetCall(int callId)
        {
            var call = await _context.CallSessions
                .Include(c => c.CallParticipants)
                .FirstOrDefaultAsync(c => c.Id == callId);

            if (call == null)
                return NotFound();

            return Ok(call);
        }*/
        [HttpPost("join")]
        public async Task<IActionResult> JoinCall(JoinCallDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            if (userIdClaim == null || roleClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);
            var role = roleClaim.Value;

            var session = await _context.Sessions
                .Include(s => s.AvailableSlots)
                .FirstOrDefaultAsync(s => s.id == dto.SessionId);

            if (session == null)
                return NotFound("Session not found");

            if (session.AvailableSlots == null)
                return BadRequest("Session slot not found");

            var slot = session.AvailableSlots;

            var isDoctor = role == "Doctor" && session.DoctorId == userId;
            var isPatient = role == "Patient" && session.UserId == userId;

            var isGroup = slot.Type == SlotType.Group;

            // In group sessions, each patient has a different SessionId,
            // but they share the same AvailableSlotsId.
            var isBookedGroupPatient = false;

            if (isGroup && role == "Patient")
            {
                isBookedGroupPatient = await _context.Sessions.AnyAsync(s =>
                    s.AvailableSlotsId == slot.id &&
                    s.UserId == userId
                );
            }

            if (!isGroup)
            {
                if (!isDoctor && !isPatient)
                    return Unauthorized("You are not part of this session");
            }
            else
            {
                if (!isDoctor && !isBookedGroupPatient)
                    return Unauthorized("You are not booked in this group session");
            }

            // --- حساب الوقت والسماحية ---
            var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var nowInEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);

            TimeSpan slotTime;

            if (!TimeSpan.TryParse(slot.time, out slotTime))
            {
                if (!DateTime.TryParse(slot.time, out var parsedTime))
                    return BadRequest("Invalid time format in slot");

                slotTime = parsedTime.TimeOfDay;
            }

            var sessionStartDateTime = slot.date.Date.Add(slotTime);
            var sessionEndDateTime = sessionStartDateTime.AddMinutes(60);

            // السماحية: يقدر يخش قبلها بـ 30 دقيقة
            var allowedStartDateTime = sessionStartDateTime.AddMinutes(-30);

            // لو بيحاول يخش قبل السماحية
            if (nowInEgypt < allowedStartDateTime)
            {
                return BadRequest(new
                {
                    code = "SESSION_NOT_STARTED",
                    message = "Session has not started yet. You can join 30 minutes before the scheduled time.",
                    currentTime = nowInEgypt.ToString("yyyy-MM-dd HH:mm"),
                    startTime = sessionStartDateTime.ToString("yyyy-MM-dd HH:mm"),
                    allowedToJoinAt = allowedStartDateTime.ToString("yyyy-MM-dd HH:mm")
                });
            }

            // لو بيحاول يخش بعد ما الجلسة خلصت
            if (nowInEgypt > sessionEndDateTime)
            {
                return BadRequest(new
                {
                    code = "SESSION_ENDED",
                    message = "Session has ended.",
                    currentTime = nowInEgypt.ToString("yyyy-MM-dd HH:mm"),
                    endTime = sessionEndDateTime.ToString("yyyy-MM-dd HH:mm")
                });
            }
            // --- نهاية حساب الوقت ---

            CallSession? call;

            if (isGroup)
            {
                // Same room for all patients booked in the same group slot
                call = await _context.CallSessions
                    .Include(c => c.Session)
                    .Include(c => c.CallParticipants)
                    .FirstOrDefaultAsync(c =>
                        c.Session.AvailableSlotsId == slot.id &&
                        c.Status != "Ended"
                    );
            }
            else
            {
                // One-to-one call is linked to the exact session
                call = await _context.CallSessions
                    .Include(c => c.CallParticipants)
                    .FirstOrDefaultAsync(c =>
                        c.SessionId == session.id &&
                        c.Status != "Ended"
                    );
            }

            if (call == null)
            {
                call = new CallSession
                {
                    SessionId = session.id,
                    RoomId = Guid.NewGuid().ToString(),
                    IsGroupCall = isGroup,
                    MaxParticipants = isGroup ? 10 : 2,
                    Status = "Started",
                    IsStarted = true
                };

                _context.CallSessions.Add(call);
                await _context.SaveChangesAsync();

                call.CallParticipants = new List<CallParticipant>();
            }

            var participantExists = call.CallParticipants.Any(p =>
                p.UserId == userId &&
                p.IsDoctor == isDoctor
            );

            if (!participantExists)
            {
                if (isGroup && !isDoctor)
                {
                    var patientParticipantsCount = call.CallParticipants.Count(p => !p.IsDoctor);

                    if (patientParticipantsCount >= call.MaxParticipants)
                        return BadRequest("The group session is full");
                }

                _context.CallParticipants.Add(new CallParticipant
                {
                    CallSessionId = call.Id,
                    UserId = userId,
                    IsDoctor = isDoctor
                });

                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                roomId = call.RoomId,
                callId = call.Id,
                isGroup = call.IsGroupCall,
                isDoctor = isDoctor,
                status = call.Status,
                sessionStart = sessionStartDateTime.ToString("yyyy-MM-dd HH:mm"),
                sessionEnd = sessionEndDateTime.ToString("yyyy-MM-dd HH:mm")
            });
        }

        //  إنهاء المكالمة (للدكتور فقط)
        // 🟢 4) End Call
        [HttpPost("end/{callId}")]
        public async Task<IActionResult> EndCall(int callId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var call = await _context.CallSessions
                .Include(c => c.Session)
                .FirstOrDefaultAsync(c => c.Id == callId);

            if (call == null)
                return NotFound("Call not found");

            if (call.Session.DoctorId != userId)
                return Unauthorized("Only doctor can end");

            // تحديث حالة المكالمة
            call.Status = "Ended";
            call.IsStarted = false;

            // 👈 السطر الجديد: تحديث حالة الجلسة الأصلية لـ Completed
            if (call.Session != null)
            {
                call.Session.Status = "Completed";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Call ended and session marked as completed"
            });
        }

        //  جلب بيانات المكالمة (لو احتاجوا يعرضوا مين موجود دلوقتي)
        [HttpGet("{callId}")]
        public async Task<IActionResult> GetCallInfo(int callId)
        {
            var call = await _context.CallSessions
                .Include(c => c.CallParticipants)
                .FirstOrDefaultAsync(c => c.Id == callId);

            if (call == null) return NotFound();

            return Ok(call);
        }
    }
}