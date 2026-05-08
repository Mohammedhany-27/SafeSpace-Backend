using System.ComponentModel.DataAnnotations;

namespace safespace.DTOs
{
    public class RegisterPatientDto
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string DisplayName { get; set; } = "";

        [Required ,EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, special character and be at least 8 characters."
        )]
        public string Password { get; set; }

        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        [Range(1, 120)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; } = "";
    }
}