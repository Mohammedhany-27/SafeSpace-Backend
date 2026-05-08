namespace safespace.DTOs
{
    public class AddDoctorSlotDto
    {
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;

        // اكتب OneToOne أو Group
        public string Type { get; set; } = "OneToOne";
    }
}