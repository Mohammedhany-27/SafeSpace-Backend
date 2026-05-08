namespace safespace.DTOs
{
    public class JoinCallDto
    {
        public int SessionId { get; set; }
        public bool IsGroupCall { get; set; } = false;
   
        public int CallSessionId { get; set; }
    }
}
