using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class DoctorDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DoctorDashboardController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("MyProfile")]
        public async Task<IActionResult> MyProfile()
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            var doctor = await _context.Doctor
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
                return NotFound("Doctor not found");

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            return Ok(new
            {
                doctor.Id,
                doctor.FullName,
                doctor.Email,
                doctor.Position,
                doctor.Specialization,
                doctor.YearOfExperience,
                doctor.About,
                doctor.AboutSession,
                doctor.TherapyApproach,
                doctor.Rating,
                doctor.ReviewsCount,
                ImageUrl = string.IsNullOrEmpty(doctor.ProfileImageUrl)
                    ? null
                    : baseUrl + doctor.ProfileImageUrl
            });
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] DoctorUpdateProfileDto dto)
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (doctorIdClaim == null)
                return Unauthorized();

            var doctorId = int.Parse(doctorIdClaim.Value);

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
                return NotFound("Doctor not found");

            // شلنا كل الـ if بتاعة الاسم والتخصص وسبنا الصورة بس

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(dto.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return BadRequest("Only jpg, jpeg, png allowed");

                if (dto.Image.Length > 3 * 1024 * 1024)
                    return BadRequest("Image size must be less than 3MB");

                var imagesFolder = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagesFolder))
                    Directory.CreateDirectory(imagesFolder);

                if (!string.IsNullOrEmpty(doctor.ProfileImageUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, doctor.ProfileImageUrl.TrimStart('/'));

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var path = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                doctor.ProfileImageUrl = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();

            return Ok("Doctor profile updated successfully");
        }
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto dto)
        {
            var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var doctor = await _context.Doctor.FindAsync(doctorId);

            if (doctor == null) return NotFound();

            // التأكد إن الباسورد الجديد وتأكيده متطابقين
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return BadRequest("Passwords do not match");

            var hasher = new PasswordHasher<DoctorProfile>();

            // التأكد إن الباسورد الحالي صح
            var result = hasher.VerifyHashedPassword(doctor, doctor.PasswordHash, dto.CurrentPassword);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Current password is incorrect");

            // تشفير وحفظ الباسورد الجديد
            doctor.PasswordHash = hasher.HashPassword(doctor, dto.NewPassword);
            await _context.SaveChangesAsync();

            return Ok("Password updated successfully");
        }
    }
}