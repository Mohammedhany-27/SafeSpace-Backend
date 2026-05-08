using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using safespace.Model;
using safespace.Data;
using safespace.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SafeSpace.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : ControllerBase
    {
        //Dependence
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        //private readonly UserManager<User> _userManager;

        //constructor
        public PatientController(AppDbContext context , IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            //_userManager = userManager;
        }


        [HttpGet("MyProfile")]
        public async Task<IActionResult> GetProfile()
        {
            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            /* مؤقتًا للتجربة فقط
            var userId = 1; // نستخدم Dummy UserId*/

            var patient = await _context.PatientProfile
                .AsNoTracking()
                //.Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == PatientId);

            if (patient == null)
                return NotFound();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var dto = new UserProfileDto
            {
                FullName = patient.FullName,
                DisplayName= patient.DisplayName,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Location = patient.Location ,
                ImageUrl = string.IsNullOrEmpty(patient.ProfileImageUrl)
                    ? null
                    : baseUrl + patient.ProfileImageUrl,
                MemberSince = "Member since " + patient.MemberSince.ToString("MMM yyyy"),
                TotalSessions = patient.TotalSessions,
                ActiveStreak = patient.ActiveStreakWeeks + " weeks",
                WellnessScore = patient.WellnessScore + "%",
                EmailNotifications = patient.EmailNotifications,
                SmsReminders = patient.SMSReminders
            };

            return Ok(dto);
        }
        /*
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("No image uploaded");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only jpg, jpeg, png allowed");

            if (image.Length > 2 * 1024 * 1024)
                return BadRequest("Image size must be less than 2MB");

            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            var patient = await _context.PatientProfile
                .FirstOrDefaultAsync(p => p.Id == PatientId);

            if (patient == null)
                return NotFound();

            var imagesFolder = Path.Combine(_environment.WebRootPath, "images");

            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            // حذف الصورة القديمة
            if (!string.IsNullOrEmpty(patient.ProfileImageUrl))
            {
                var oldPath = Path.Combine(_environment.WebRootPath, patient.ProfileImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            var fileName = Guid.NewGuid().ToString() + extension;
            var path = Path.Combine(imagesFolder, fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            patient.ProfileImageUrl = "/images/" + fileName;

            await _context.SaveChangesAsync();

            return Ok(patient.ProfileImageUrl);
        }*/

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            var patient = await _context.PatientProfile
                .FirstOrDefaultAsync(p => p.Id == PatientId);

            if (patient == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.FullName))
                patient.FullName = dto.FullName;

            if (!string.IsNullOrEmpty(dto.DisplayName))
                patient.DisplayName = dto.DisplayName;

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
                patient.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrEmpty(dto.Location))
                patient.Location = dto.Location;

            if (dto.EmailNotifications.HasValue)
                patient.EmailNotifications = dto.EmailNotifications.Value;

            if (dto.SmsReminders.HasValue)
                patient.SMSReminders = dto.SmsReminders.Value;

            // رفع صورة جديدة
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

                //حذف الصورة القديمة
                if (!string.IsNullOrEmpty(patient.ProfileImageUrl))
                {
                    var oldPath = Path.Combine(_environment.WebRootPath, patient.ProfileImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var path = Path.Combine(imagesFolder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                patient.ProfileImageUrl = "/images/" + fileName;
            }

            await _context.SaveChangesAsync();

            return Ok("Profile updated successfully");
        }
        
        [HttpPost("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] ChangePasswordDto dto)
        {
            var PatientIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            if (PatientIdClaim == null)
                return Unauthorized();

            var PatientId = int.Parse(PatientIdClaim.Value);

            var patient = await _context.PatientProfile
                .FirstOrDefaultAsync(p => p.Id == PatientId);

            if (patient == null)
                return NotFound();

            if (dto.ConfirmNewPassword != dto.ConfirmNewPassword)
                return BadRequest("passwords not match");

            var hasher = new PasswordHasher<PatientProfile>();
            var password = hasher.VerifyHashedPassword(patient, patient.PasswordHash, dto.CurrentPassword);

            if (password == PasswordVerificationResult.Failed)
                return BadRequest("current password is incorrect");

            patient.PasswordHash = hasher.HashPassword(patient, dto.NewPassword);

            _context.PatientProfile.Update(patient);
            await _context.SaveChangesAsync();

            return Ok("password change successfully");
        }
    }
}

