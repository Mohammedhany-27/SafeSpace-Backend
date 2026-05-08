using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using safespace.Data;
using safespace.DTOs;
using safespace.Model;
using Microsoft.EntityFrameworkCore;

namespace safespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly safespace.Services.EmailService _emailService;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context,
                      safespace.Services.EmailService emailService,
                      IConfiguration config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        // Register Patient
        [HttpPost("register")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterPatientDto model)
        {
            Console.WriteLine("REGISTER HIT");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.PatientProfile.Any(p => p.Email == model.Email))
                return BadRequest("Email already exists");

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var token = Guid.NewGuid().ToString();
            var hasher = new PasswordHasher<PatientProfile>();


            var patient = new PatientProfile
            {
                FullName = model.FullName,
                DisplayName = model.DisplayName,
                Email = model.Email,
                //PasswordHash = model.Password,
                Age = model.Age,
                Gender = model.Gender,
                EmailVerificationToken = token,
                TokenExpiry = DateTime.UtcNow.AddHours(24),
                IsEmailVerified = false,

                PasswordHash = hasher.HashPassword(null, model.Password)
            };

            //var hasher = new PasswordHasher<PatientProfile>();
            //patient.PasswordHash = hasher.HashPassword(patient, model.Password);

            _context.PatientProfile.Add(patient);
            await _context.SaveChangesAsync();

            var verificationLink =$"{Request.Scheme}://{Request.Host}/api/auth/verify-email?token={token}";
            //Console.WriteLine(token);
            //Console.WriteLine(verificationLink);
            _emailService.SendVerificationEmail(model.Email, verificationLink);

            return Ok("Account created. Please verify your email.");
        }

        // Verify Email
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            // البحث عن المستخدم باستخدام التوكن
            var user = await _context.PatientProfile
                .FirstOrDefaultAsync(p => p.EmailVerificationToken == token);

            if (user == null)
                // إذا لم يجد التوكن، قد يكون مفعلاً بالفعل، تأكدي من قاعدة البيانات
                return BadRequest("Invalid token or email already verified.");

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null; // نمسح التوكن بعد التفعيل للامان

            await _context.SaveChangesAsync();
            return Ok("Email verified successfully");
        }

        // Login
        // Login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // ==========================================
            // فحص جدول الأدمن أولاً
            // ==========================================
            var admin = _context.Admins
                .FirstOrDefault(a => a.Email.Trim() == model.Email.Trim());

            if (admin != null)
            {
                var adminHasher = new PasswordHasher<AdminProfile>();

                // 🟢 نفس اللوجيك بتاعك للتعامل مع الباسورد العادي والمتشفر
                if (admin.PasswordHash.StartsWith("AQAAAA"))
                {
                    var adminResult = adminHasher.VerifyHashedPassword(admin, admin.PasswordHash, model.Password);
                    if (adminResult == PasswordVerificationResult.Failed)
                        return Unauthorized("Invalid Password");
                }
                else
                {
                    // لو الباسورد مش متشفر (زي ما ضفناه في الـ SQL)
                    if (admin.PasswordHash != model.Password)
                        return Unauthorized("Invalid Password");

                    // هنشفره ونسيفه عشان المرات الجاية
                    admin.PasswordHash = adminHasher.HashPassword(admin, model.Password);
                    _context.SaveChanges();
                }

                var adminClaims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, admin.Id.ToString()),
                    new Claim(ClaimTypes.Email, admin.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var adminKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                var adminCreds = new SigningCredentials(
                    adminKey, SecurityAlgorithms.HmacSha256);

                var adminToken = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: adminClaims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: adminCreds
                );

                var adminJwt = new JwtSecurityTokenHandler().WriteToken(adminToken);

                return Ok(new
                {
                    token = adminJwt,
                    role = "Admin",
                    user = new
                    {
                        admin.Id,
                        admin.Email
                    }
                });
            }
            // ==========================================
            // نهاية فحص الأدمن
            // ==========================================

            var user = _context.PatientProfile
                .AsNoTracking()
                .FirstOrDefault(p => p.Email.Trim() == model.Email.Trim());

            if (user == null)
            {
                var doctor = _context.Doctor
                    .FirstOrDefault(d => d.Email.Trim() == model.Email.Trim());

                if (doctor == null)
                    return Unauthorized("Invalid Email");

                if (!doctor.IsActive)
                    return Unauthorized("Doctor account inactive");

                var doctorHasher = new PasswordHasher<DoctorProfile>();

                if (doctor.PasswordHash.StartsWith("AQAAAA"))
                {
                    var doctorResult = doctorHasher.VerifyHashedPassword(
                        doctor,
                        doctor.PasswordHash,
                        model.Password
                    );

                    if (doctorResult == PasswordVerificationResult.Failed)
                        return Unauthorized("Invalid Password");
                }
                else
                {
                    if (doctor.PasswordHash != model.Password)
                        return Unauthorized("Invalid Password");

                    doctor.PasswordHash = doctorHasher.HashPassword(doctor, model.Password);
                    _context.SaveChanges();
                }

                var doctorClaims = new[]
                {
        new Claim(ClaimTypes.NameIdentifier, doctor.Id.ToString()),
        new Claim(ClaimTypes.Email, doctor.Email),
        new Claim(ClaimTypes.Name, doctor.FullName),
        new Claim(ClaimTypes.Role, "Doctor")
    };

                var doctorKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                var doctorCreds = new SigningCredentials(
                    doctorKey, SecurityAlgorithms.HmacSha256);

                var doctorToken = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: doctorClaims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: doctorCreds
                );

                var doctorJwt = new JwtSecurityTokenHandler().WriteToken(doctorToken);

                return Ok(new
                {
                    token = doctorJwt,
                    role = "Doctor",
                    user = new
                    {
                        doctor.Id,
                        doctor.FullName,
                        doctor.Email
                    }
                });
            }

            if (!user.IsEmailVerified)
            {

                var User = _context.PatientProfile.FirstOrDefault(p => p.Id == user.Id);
                if (User != null)
                {
                    user.IsEmailVerified = User.IsEmailVerified;
                }
                //return Unauthorized("Please verify your email first");
            }

            /*user.IsEmailVerified = true;
            _context.SaveChanges();*/
            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
                return Unauthorized("Account locked. Try again later.");

            var hasher = new PasswordHasher<PatientProfile>();
            /*
            var result = hasher.VerifyHashedPassword( user,user.PasswordHash,model.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= 5)
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                _context.SaveChanges();

                return Unauthorized("Invalid Password");
            }
            */
            if (user.PasswordHash.StartsWith("AQAAAA"))
            {
                var result = hasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    model.Password
                );

                if (result == PasswordVerificationResult.Failed)
                {
                    user.FailedLoginAttempts++;

                    if (user.FailedLoginAttempts >= 5)
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                    _context.SaveChanges();

                    return Unauthorized("Invalid Password");
                }
            }
            else
            {
                if (user.PasswordHash != model.Password)
                {
                    user.FailedLoginAttempts++;

                    if (user.FailedLoginAttempts >= 5)
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);

                    _context.SaveChanges();

                    return Unauthorized("Invalid Password");
                }

                user.PasswordHash = hasher.HashPassword(user, model.Password);
            }
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            _context.SaveChanges();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, "Patient")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = jwt,
                role = "Patient",
                user = new
                {
                    user.Id,
                    user.FullName,
                    user.Email
                }
            });
        }
    }
    }