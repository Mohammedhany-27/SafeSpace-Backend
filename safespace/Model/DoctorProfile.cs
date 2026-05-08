using System.ComponentModel.DataAnnotations;

namespace safespace.Model
{
    public class DoctorProfile
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string Position { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public int YearOfExperience { get; set; } = 0;

        [Range(1, 5)]
        public double Rating { get; set; } = 0.0;

        public int ReviewsCount { get; set; } = 0;
        public string AboutSession { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string TherapyApproach { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public ICollection<Certification> Certification { get; set; } = new List<Certification>();
        public ICollection<Review> Review { get; set; } = new List<Review>();
    }
}