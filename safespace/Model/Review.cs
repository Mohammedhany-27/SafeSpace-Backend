
using System.ComponentModel.DataAnnotations;

namespace safespace.Model
{
    public class Review
    {
        public int Id { get; set; }
        public int? PatientProfileId { get; set; }
        public virtual PatientProfile PatientProfile { get; set; } = null!;
        public int DoctorId { get; set; }
        public virtual DoctorProfile Doctor { get; set; } = null!;

        //[Range(1,5)]
        public int ReviewValue { get; set; } = 0;

        //[MaxLength(100)]
        public string ReviewDescription { get; set;} = string.Empty;
        public DateTime CreatedAt { get; set; } = Date.GetEgyptTime();
    }
}
