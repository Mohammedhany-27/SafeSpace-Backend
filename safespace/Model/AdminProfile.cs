namespace safespace.Model
{
    public class AdminProfile
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        // ممكن تضيف أي بيانات تانية للأدمن هنا مستقبلاً (مثل الاسم)
    }
}