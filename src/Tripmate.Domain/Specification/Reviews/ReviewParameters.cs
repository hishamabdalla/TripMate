using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.Reviews
{
    public class ReviewParameters
    {
        public int? EntityId { get; set; }
        public ReviewType? ReviewType { get; set; }
        public string? UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}

