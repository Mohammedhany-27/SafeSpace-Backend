using System.Text.Json.Serialization;
namespace safespace.DTOs
{
    public class UserProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string DisplayName { get; set; }= string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }

        public string? ImageUrl { get; set; }
        public string MemberSince { get; set; } = string.Empty;

        public int TotalSessions { get; set; } = 0;
        public string ActiveStreak { get; set; } = "0 weeks";
        public string WellnessScore { get; set; } = "0%";
        public bool EmailNotifications { get; set; } = true;
        public bool SmsReminders { get; set; } = true;
        //public List<SessionsDTO> UpcomingSessions { get; set; } = new List<SessionsDTO>();
    }

    public class UpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public IFormFile? Image { get; set; }
        public bool? EmailNotifications { get; set; }
        public bool? SmsReminders { get; set; }
    }
    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
