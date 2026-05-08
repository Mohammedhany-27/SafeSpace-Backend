namespace safespace.Model
{
    public class CallParticipant
    {
        public int Id { get; set; }

        public int CallSessionId { get; set; }
        public CallSession CallSession { get; set; }

        public int UserId { get; set; }

        public bool IsDoctor { get; set; } = false;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}