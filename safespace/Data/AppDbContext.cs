//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using safespace.Model;
namespace safespace.Data
{
    public class AppDbContext : DbContext
    {
        //Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // أي علاقة في قاعدة البيانات، لو اتمسح الأب، الابن متمسحوش أوتوماتيك
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        public DbSet<User> User { get; set; }
        public DbSet<PatientProfile> PatientProfile { get; set; }
        //public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorProfile> Doctor { get; set; }
        public DbSet<Chats> Chat { get; set; }
        //public DbSet<Chat> ChatParticipants { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Certification> Certification { get; set; }
        public DbSet<AvailableSlots> AvailableSlots { get; set; }
        public DbSet<Sessions> Sessions { get; set; }
        public DbSet<CallSession> CallSessions { get; set; }
        public DbSet<CallParticipant> CallParticipants { get; set; }
        public DbSet<AdminProfile> Admins { get; set; }
    }
}

