using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using safespace.Data;
using safespace.DTOs;
using safespace.Model;

namespace safespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Uncomment this when you implement the Admin role
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Add new doctor (from Dashboard)
        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] RegisterDoctorDto dto)
        {
            if (await _context.Doctor.AnyAsync(d => d.Email == dto.Email))
                return BadRequest("Email is already registered to another doctor");

            var hasher = new PasswordHasher<DoctorProfile>();
            var doctor = new DoctorProfile
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Specialization = dto.Specialization,
                IsActive = true,
                PasswordHash = "",

                Position = dto.Position ?? "",
                YearOfExperience = dto.YearOfExperience ?? 0,
                About = dto.Bio ?? "",
                AboutSession = dto.AboutSession ?? "",
                TherapyApproach = dto.TherapyApproach ?? ""
            };

            doctor.PasswordHash = hasher.HashPassword(doctor, dto.Password);

            _context.Doctor.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Doctor added successfully", doctorId = doctor.Id });
        }

        // 2. Update doctor info by Admin
        [HttpPut("UpdateDoctorInfo/{id}")]
        public async Task<IActionResult> UpdateDoctorInfo(int id, [FromBody] AdminUpdateDoctorDto dto)
        {
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null) return NotFound("Doctor not found");

            if (!string.IsNullOrEmpty(dto.FullName)) doctor.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Position)) doctor.Position = dto.Position;
            if (!string.IsNullOrEmpty(dto.Specialization)) doctor.Specialization = dto.Specialization;
            if (dto.YearOfExperience.HasValue) doctor.YearOfExperience = dto.YearOfExperience.Value;
            if (!string.IsNullOrEmpty(dto.About)) doctor.About = dto.About;
            if (!string.IsNullOrEmpty(dto.AboutSession)) doctor.AboutSession = dto.AboutSession;
            if (!string.IsNullOrEmpty(dto.TherapyApproach)) doctor.TherapyApproach = dto.TherapyApproach;

            await _context.SaveChangesAsync();
            return Ok("Doctor profile updated successfully");
        }

        // 3. Get all doctors for Dashboard table
        [HttpGet("GetAllDoctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _context.Doctor
                .Select(d => new
                {
                    d.Id,
                    d.FullName,
                    d.Email,
                    d.Specialization,
                    d.IsActive,
                    d.Rating
                })
                .ToListAsync();

            return Ok(doctors);
        }

        // 4. Toggle doctor status (Kill switch)
        [HttpPatch("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var doctor = await _context.Doctor.FindAsync(id);
            if (doctor == null) return NotFound("Doctor not found");

            doctor.IsActive = !doctor.IsActive;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Doctor status updated successfully", isActive = doctor.IsActive });
        }
    }
}