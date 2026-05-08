using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using safespace.Data;
using safespace.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using safespace.Model;
using Microsoft.Extensions.Options;
using SafeSpace.Hubs;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost",
              policy =>
              {
                  //policy.WithOrigins("http://localhost:5173", "http://localhost:5177", "http://localhost:5175", "http://localhost:7134", "http://localhost:7131", "http://localhost:3000")
                   policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                  .AllowAnyHeader()
                   .AllowAnyMethod();
              });
        });


        /*// 1. إضافة الـ Database في الذاكرة
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("SafeSpaceTestDB"));

        // 2. إعداد الـ Authentication بشكل صحيح (تصحيح خطأ السطر 26)
        var key = builder.Configuration["JWT:Key"] ?? "DefaultSecretKey1234567890123456";
        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });*/

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddScoped<safespace.Services.EmailService>();
        builder.Services.AddHostedService<safespace.Services.MessageService>();


        // تعريف المفتاح مع حماية في حالة فشل قراءة ملف appsettings
        //var jwtKey = builder.Configuration["Jwt:Key"];
        var jwtKey = builder.Configuration.GetSection("Jwt")["Key"];
        Console.WriteLine("JWT KEY = " + jwtKey);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
        });


        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        //builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Enter: Bearer YOUR_TOKEN"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
        });


        builder.Services.AddSignalR();

        var app = builder.Build();

        app.UseStaticFiles();

        app.UseRouting();

        /*using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

             //Dummy User
            context.PatientProfile.Add(new PatientProfile
            {
                UserId = 1,
                PhoneNumber = "01000000000",

                Location = "Test City",
                ProfileImageUrl = "/images/default.png",
                EmailNotifications = true,
                SMSReminders = false,
                MemberSince = DateTime.Now.AddMonths(-3),
                TotalSessions = 5,
                ActiveStreakWeeks = 2,
                WellnessScore = 85
            });

            context.SaveChanges();
        }*/

        /*using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1. إضافة مستخدم مرتب بالفئة (User)
            if (!context.User.Any(u => u.Id == 1))
            {
                context.User.Add(new User
                {
                    Id = 1,
                    FullName = "دكتور أحمد محمد",
                    Email = "dr@test.com"
                });
            }

             //2. إضافة بروفايل الدكتور (DoctorProfile)
            if (!context.DoctorProfile.Any(d => d.Id == 1))
            {
                context.DoctorProfile.Add(new DoctorProfile
                {
                    Id = 1,
                    UserId = 1,
                    Specialization = "Psychiatrist",
                    YearOfExperience = 10,
                    Rating = 4.5,
                    ReviewsCount = 20,
                    About = "متخصص في العلاج النفسي السلوكي",
                    AboutSession = "جلسات مريحة وخصوصية تامة",
                    TherapyApproach = "CBT",
                    // إضافة شهادة تجريبية لتجربة الـ Include
                    Certification = new List<Certification>
                    {
                        new Certification { Title = "MBBCh من جامعة القاهرة" }
                    },
                    Review = new List<Review>
                    {
                        new Review { ReviewDescription = "love this doctor",ReviewValue = 4 },
                        new Review { ReviewDescription = "not bad",ReviewValue = 3 },
                        new Review { ReviewDescription = "this is greet",ReviewValue = 5 }
                    }
                });
            }

            context.SaveChanges();
        }*/

        // Configure the HTTP request pipeline.
        /*if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }*/
        app.UseDeveloperExceptionPage();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        //app.UseCors("AllowFrontend");
        app.UseCors("AllowLocalhost");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<ChatHub>("/chatHub");
        app.MapHub<SessionHub>("/hubs/sessions");
        app.MapHub<CallHub>("/callHub");
        app.Run();
    }
}