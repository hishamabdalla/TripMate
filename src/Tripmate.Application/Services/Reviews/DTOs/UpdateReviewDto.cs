namespace Tripmate.Application.Reviews.DTOs
{
    public class UpdateReviewDto
    {
        public int Id { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}

