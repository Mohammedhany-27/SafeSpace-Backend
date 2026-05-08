using System.ComponentModel.DataAnnotations;
using System.Data;

namespace safespace.Model
{
    public class PatientProfile:User
    {
        //public int Id { get; set; }
        [Required]
        public string DisplayName { get; set; } = "";

        /*public int UserId { get; set; }
        public User User { get; set; } = null!;
        */
        [Required]
        public int Age { get; set; }

        [Required]
        public required string Gender { get; set; }
        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime MemberSince { get; set; } = Date.GetEgyptTime();
        public int TotalSessions { get; set; } = 0;
        public int ActiveStreakWeeks { get; set; } = 0;
        public double WellnessScore { get; set; } = 0.0;
        public bool IsEmailVerified { get; set; } = false;
        public string? EmailVerificationToken { get; set; }
        public bool EmailNotifications { get; set; } = true;
        public bool SMSReminders { get; set; } = true ;
        //public ICollection<Appointment> Appointment { get; set; } = new List<Appointment>();
    }
}
