using System.Numerics;

namespace safespace.Model
{
    public class Certification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int DoctorId { get; set; }
        public DoctorProfile Doctor { get; set; }
    }
}
