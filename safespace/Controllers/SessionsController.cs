using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using safespace.Data;
using safespace.DTOs;
using SafeSpace.Hubs;
using safespace.Migrations;
using safespace.Model;
using System.Security.Claims;

namespace safespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : ControllerBase
    {
        //Dependence
        private readonly AppDbContext _context;
        private readonly IHubContext<SessionHub> _hub;
        //constructor
        public SessionsController(AppDbContext context, IHubContext<SessionHub> hub)
        {
            _context = context;
            _hub = hub;
        }
        [HttpGet("AvailableSlots")]
        public async Task<IActionResult> GetSlots(int DoctorId, DateTime date, SlotType type)
        {
            var AvailableSlots = await _context.AvailableSlots
                .Where(d => d.DoctorId == DoctorId && d.date.Date == date.Date && d.Type == type)
                .OrderBy(d => d.time)
                .Select(d => new AvailableSlotsDTO
                {
                    AvailableSlotsId = d.id,
                    time = d.time,
                    IsBooked = d.IsBooked,
                    SlotType = d.Type.ToString()
                })
                .ToListAsync();
            return Ok(AvailableSlots);

        }
        [Authorize]
        [HttpPost("Book")]
        public async Task<IActionResult> BookSession(BookDTO dto)
        {
            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            var slotInfo = await _context.AvailableSlots.AsNoTracking()
                .FirstOrDefaultAsync(s => s.id == dto.AvailableSlotsId);

            if (slotInfo == null)
                return NotFound();

            if (slotInfo.IsBooked)
                return BadRequest("Slot already booked");

            int Book = 0;

            if (slotInfo.Type == SlotType.OneToOne)
            {
                Book = await _context.AvailableSlots
                    .Where(s => s.id == dto.AvailableSlotsId && !s.IsBooked)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsBooked, true));
            }
            else
            {
                Book = await _context.AvailableSlots
                    .Where(s => s.id == dto.AvailableSlotsId && !s.IsBooked && s.CurrentCapacity < 10)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.CurrentCapacity, p => p.CurrentCapacity + 1)
                        .SetProperty(p => p.IsBooked, p => (p.CurrentCapacity + 1) >= 10));
            }

            // Race Condition
            if (Book == 0)
                return BadRequest("Slot already booked");

            var AvailableSlots = await _context.AvailableSlots
                .FirstAsync(s => s.id == dto.AvailableSlotsId);
            var sessions = new Sessions
            {
                DoctorId = AvailableSlots.DoctorId,
                UserId = PatientId,
                AvailableSlotsId = AvailableSlots.id,
                Status = "Booked"
            };

            _context.Sessions.Add(sessions);
            await _context.SaveChangesAsync();

            //Realtime
            if (AvailableSlots.IsBooked)
            {
                await _hub.Clients.Group($"Doctor_{AvailableSlots.DoctorId}")
                .SendAsync("SlotBooked", AvailableSlots.id);
            }
            return Ok(sessions);
        }
        [Authorize]
        [HttpGet("MySessions")]
        public async Task<IActionResult> MySession()
        {
            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            var sessions = await _context.Sessions
                .Where(p => p.UserId == PatientId)
                .Include(p => p.AvailableSlots)
                .Include (p => p.Doctor)
                .ToListAsync();
            var Upcoming = sessions.Where(s => s.AvailableSlots.date >= Date.GetEgyptTime())
                .OrderBy(s => s.AvailableSlots.date)
                .Select(s => new sessionsDTO 
                { 
                  SessionsId = s.id, 
                  DoctorName = s.Doctor.FullName,
                  date=s.AvailableSlots.date, 
                  time = s.AvailableSlots.time,
                  SessionType = s.AvailableSlots.Type.ToString()
                }).ToList();
            var Past = sessions.Where(s => s.AvailableSlots.date < Date.GetEgyptTime())
                .OrderByDescending(s => s.AvailableSlots.date)
                .Select(s => new sessionsDTO
                {
                    SessionsId = s.id,
                    DoctorName = s.Doctor.FullName,
                    date = s.AvailableSlots.date,
                    time = s.AvailableSlots.time,
                    SessionType = s.AvailableSlots.Type.ToString()
                }).ToList();
            return Ok(new MySessionsDTO { Past= Past, Upcoming = Upcoming });
        }
        [Authorize]
        [HttpPost("reschedule")]
        public async Task<IActionResult> Reschedule(RescheduleDto dto)
        {
            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            var sessions = await _context.Sessions
                .FirstOrDefaultAsync(s => s.id == dto.SessionsId && s.UserId == PatientId);

            if (sessions == null)
                return NotFound();

            var oldSlot = await _context.AvailableSlots
                .FirstOrDefaultAsync(s => s.id == sessions.AvailableSlotsId);

            var newSlot = await _context.AvailableSlots
                .FirstOrDefaultAsync(s => s.id == dto.NewAvilableSlotsId);

            if (oldSlot == null || newSlot == null)
                return NotFound();

            if (newSlot.IsBooked)
                return BadRequest("Slot already booked");

            if (oldSlot.Type == SlotType.Group)
            {
                oldSlot.CurrentCapacity--;
                oldSlot.IsBooked = false;
            }
            else
            {
                oldSlot.IsBooked = false;
            }

            if (newSlot.Type == SlotType.Group)
            {
                newSlot.CurrentCapacity++;
                if (newSlot.CurrentCapacity >= 10) 
                    newSlot.IsBooked = true;
            }
            else
            {
                newSlot.IsBooked = true;
            }

            sessions.AvailableSlotsId = newSlot.id;

            await _context.SaveChangesAsync();
            // تنبيه الـ SignalR للموعد القديم (بقى متاح) والموعد الجديد (بقى محجوز)
            await _hub.Clients.Group($"Doctor_{sessions.DoctorId}")
                .SendAsync("SlotReleased", oldSlot.id);

            await _hub.Clients.Group($"Doctor_{sessions.DoctorId}")
                .SendAsync("SlotBooked", newSlot.id);

            return Ok(sessions);
        }
    }
}
