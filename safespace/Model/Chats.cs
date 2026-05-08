using System.ComponentModel.DataAnnotations.Schema;

namespace safespace.Model
{
    public class Chats
    {
        public int id { set; get; }
        public int DoctorId { get; set; }
        public virtual DoctorProfile Doctor { get; set; }
        public int PatientProfileId { get; set; }
        [ForeignKey("PatientProfileId")]
        public virtual PatientProfile User { get; set; }
        public string? LastMessage { get; set; }
        public DateTime LastMessageTime { get; set; } = Date.GetEgyptTime();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
