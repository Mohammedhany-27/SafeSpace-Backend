using System.ComponentModel.DataAnnotations.Schema;

namespace safespace.Model
{
    public class Message
    {
        public int id {  get; set; }
        public int chatId { get; set; }
        public virtual Chats chat { get; set; }
        public int senderId { get; set; }
        [ForeignKey("senderId")]
        public virtual User Sender { get; set; }
        public string MessageText { get; set; }=string.Empty;
        public bool isRead { get; set; }=false;
        public bool IsSaved { get; set; } = false;
        public DateTime sendAt { get; set; } = Date.GetEgyptTime();
    }
}
