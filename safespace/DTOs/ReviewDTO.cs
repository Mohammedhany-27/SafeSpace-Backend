using System.ComponentModel.DataAnnotations;

namespace safespace.DTOs
{
    public class ReviewDTO
    {
        public string UserDisplayName { get; set; } = "Anonymous User";

        public int ReviewValue { get; set; }
        public string ReviewDescription { get; set; } = string.Empty;
        public string FormattedDate { get; set; } = string.Empty;
    }

    public class CreateReviewDTO
    {
        [Range(1, 5)] public int ReviewValue { get; set; } = 0;
        [Required] public string ReviewDescribtion { get; set; } = string.Empty;
    }
}
