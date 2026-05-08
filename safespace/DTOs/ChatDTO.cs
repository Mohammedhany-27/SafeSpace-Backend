namespace safespace.DTOs
{
    public class ChatDTO
    {
        public int id {  get; set; }
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string DoctorName { get; set; }=string .Empty;
        //public string ImageUrl { get; set; } = string.Empty;
    }
    public class MessageDTO
    {
        public int chatId { get; set; }
        public string MessageText { get; set; } = string.Empty;
        public int senderId { get; set; }
    }
}
