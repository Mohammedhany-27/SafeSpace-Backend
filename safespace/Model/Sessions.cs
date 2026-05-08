namespace safespace.Model
{
    public class Sessions
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; }
        public int AvailableSlotsId { get; set; }
        public string? Notes { get; set; }
        public AvailableSlots AvailableSlots { get; set; }
        /*public enum SessionStatus { Upcoming , Past , Cancelled }
        public SessionStatus status { get; set; } = SessionStatus.Upcoming;*/
        public string Status { get; set; } = "Booked";
        public DateTime CreatedAt { get; set; } = Date.GetEgyptTime();
        

    }
}
