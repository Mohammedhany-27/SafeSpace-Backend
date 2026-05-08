using System.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace safespace.Model
{
    public abstract class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string FullName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public int FailedLoginAttempts { get; set; } = 0;
        public DateTime? LockoutEnd { get; set; }

        public string? EmailVerificationToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        //public RoleType Role { get; set; } 

        //public PatientProfile PatientProfile { get; set; }
        public DoctorProfile Doctor { get; set; }
        //public ICollection<ChatParticipants> ChatParticipants { get; set; }
    }

    //public enum RoleType { Doctor, Patient }
}
    
