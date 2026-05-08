namespace safespace.DTOs
{
    public class AvailableSlotsDTO
    {
        public int AvailableSlotsId { get; set; }
        public string time { get; set; } = string.Empty;
        public bool IsBooked { get; set; }=false;
        public string SlotType { get; set; }
    }
    public class sessionsDTO
    {
        public int SessionsId { get; set; }
        public string DoctorName { get; set; }=string.Empty;
        public DateTime date { get; set; }
        public string time { get; set; } = string.Empty;
        public string SessionType { get; set; }
    }
    public class BookDTO
    {
        //public int PatientProfileId { get; set; }
        public int AvailableSlotsId { get; set; }
    }
    public class MySessionsDTO
    {
        public List<sessionsDTO> Upcoming { get; set; }
        public List<sessionsDTO> Past { get; set; }
    }
    public class RescheduleDto
    {
        public int SessionsId { get; set; }

        public int NewAvilableSlotsId { get; set; }
    }
    namespace safespace.DTOs
    {
        public class SessionNoteDto
        {
            public string Notes { get; set; } = string.Empty;
        }
    }
}
