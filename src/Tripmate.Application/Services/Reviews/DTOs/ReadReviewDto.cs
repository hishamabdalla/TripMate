using Tripmate.Domain.Enums;

namespace Tripmate.Application.Reviews.DTOs
{
    public class ReadReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }= string.Empty;
        public string UserId { get; set; }= string.Empty;
        public ReviewType ReviewType { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }

    }
}
