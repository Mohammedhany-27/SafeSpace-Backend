namespace safespace.Model
{
    public class CallSession
    {
        public int Id { get; set; }

        public int SessionId { get; set; }
        public Sessions Session { get; set; }

        public string RoomId { get; set; } = Guid.NewGuid().ToString();

        public bool IsGroupCall { get; set; } = false;

        public bool IsStarted { get; set; } = false;

        public string Status { get; set; } = "Pending";

        public int MaxParticipants { get; set; } = 2;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CallParticipant> CallParticipants { get; set; } = new List<CallParticipant>();
    }
}