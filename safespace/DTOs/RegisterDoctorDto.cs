namespace safespace.DTOs
{
    public class RegisterDoctorDto
    {
        // بيانات الدخول والأساسيات (مطلوبة)
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;

        // البيانات المهنية (اللي الدكتور ملوش صلاحية يعدلها بعدين)
        public string? Position { get; set; }
        public int? YearOfExperience { get; set; }
        public string? Bio { get; set; } // هي الـ About في جدول الدكتور
        public string? AboutSession { get; set; }
        public string? TherapyApproach { get; set; }
    }
}