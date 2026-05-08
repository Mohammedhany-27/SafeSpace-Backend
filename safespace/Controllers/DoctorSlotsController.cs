using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using safespace.Data;
using safespace.DTOs;
using safespace.Model;
using System.Security.Claims;

namespace safespace.Controllers
{
    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorSlotsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DoctorSlotsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("AddSlot")]
        public async Task<IActionResult> AddSlot([FromBody] AddDoctorSlotDto dto)
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            var doctor = await _context.Doctor.FindAsync(doctorId);

            if (doctor == null)
                return NotFound("Doctor not found");

            if (dto.Date == default)
                return BadRequest("Date is required");

            if (string.IsNullOrWhiteSpace(dto.Time))
                return BadRequest("Time is required");

            if (!Enum.TryParse<SlotType>(dto.Type, true, out var slotType))
                return BadRequest("Invalid slot type. Use OneToOne or Group");

            TimeSpan parsedTime;

            if (!TimeSpan.TryParse(dto.Time, out parsedTime))
            {
                if (!DateTime.TryParse(dto.Time, out var parsedDateTime))
                    return BadRequest("Invalid time format. Use HH:mm like 18:00");

                parsedTime = parsedDateTime.TimeOfDay;
            }

            var slotDate = dto.Date.Date;
            var slotDateTime = slotDate.Add(parsedTime);

            var egyptZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            var nowInEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, egyptZone);

            if (slotDateTime < nowInEgypt)
                return BadRequest("Cannot add a slot in the past");

            var normalizedTime = parsedTime.ToString(@"hh\:mm");

            var exists = await _context.AvailableSlots.AnyAsync(s =>
                s.DoctorId == doctorId &&
                s.date.Date == slotDate &&
                s.time == normalizedTime &&
                s.Type == slotType
            );

            if (exists)
                return BadRequest("This slot already exists");

            var slot = new AvailableSlots
            {
                DoctorId = doctorId,
                date = slotDate,
                time = normalizedTime,
                Type = slotType,
                IsBooked = false,
                CurrentCapacity = 0
            };

            _context.AvailableSlots.Add(slot);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Slot added successfully",
                slot = new
                {
                    slot.id,
                    slot.DoctorId,
                    slot.date,
                    slot.time,
                    SlotType = slot.Type.ToString(),
                    slot.IsBooked,
                    slot.CurrentCapacity
                }
            });
        }

        [HttpGet("MySlots")]
        public async Task<IActionResult> MySlots()
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            var slots = await _context.AvailableSlots
                .Where(s => s.DoctorId == doctorId)
                .OrderBy(s => s.date)
                .ThenBy(s => s.time)
                .Select(s => new
                {
                    s.id,
                    s.date,
                    s.time,
                    SlotType = s.Type.ToString(),
                    s.IsBooked,
                    s.CurrentCapacity
                })
                .ToListAsync();

            return Ok(slots);
        }

        [HttpDelete("DeleteSlot/{slotId}")]
        public async Task<IActionResult> DeleteSlot(int slotId)
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            var slot = await _context.AvailableSlots
                .FirstOrDefaultAsync(s => s.id == slotId && s.DoctorId == doctorId);

            if (slot == null)
                return NotFound("Slot not found");

            if (slot.IsBooked || slot.CurrentCapacity > 0)
                return BadRequest("Cannot delete a booked slot");

            _context.AvailableSlots.Remove(slot);
            await _context.SaveChangesAsync();

            return Ok("Slot deleted successfully");
        }
    }
}