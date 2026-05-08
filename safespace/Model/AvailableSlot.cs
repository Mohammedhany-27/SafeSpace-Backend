namespace safespace.Model
{
    public class AvailableSlot
    {
        public int Id { get; set; }
        public int DoctorProfileId { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsBooked { get; set; } = false;
        public virtual DoctorProfile Doctor { get; set; } = null!;
    }
}
