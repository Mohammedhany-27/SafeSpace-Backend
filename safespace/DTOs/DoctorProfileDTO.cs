namespace safespace.DTOs
{
    public class DoctorProfileDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;    
        public string Specialization { get; set; } = string.Empty;
        public int YearOfExperience { get; set; }
        public double Rating { get; set; }
        public int ReviewsCount { get; set; }
        public string? ImageUrl { get; set; }

        public string AboutSession { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public string TherapyApproach { get; set; } = string.Empty;
        public List<string> Certifications { get; set; } = new List<string>();
        public List<ReviewDTO> Review { get; set; } = new List<ReviewDTO>();

        //public List<SlotDto> AvailableSlots { get; set; } = new List<SlotDto>();
    
    }
    public class DoctorCardDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get;set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        //public string TherapyApproach { get; set; } = string.Empty;
        public int YearOfExperience { get; set; }
        public string ImageUrl { get; set; }
        public double Rating { get; set; } 
        public int ReviewsCount { get; set; }
    }
}
