namespace safespace.Model
{
    public class AvailableSlots
    {
        public int id { get; set; }
        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; }
        public DateTime date { get; set; }
        public string time { get; set; }=string.Empty;
        public bool IsBooked { get; set; }=false;
        public SlotType Type { get; set; } = SlotType.OneToOne;
        public int CurrentCapacity { get; set; } = 0;
    }
    public enum SlotType { OneToOne=0 ,Group=1 }
}
