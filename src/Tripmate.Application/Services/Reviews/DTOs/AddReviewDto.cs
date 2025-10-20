using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Reviews.DTOs
{
    public class AddReviewDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
        public int EntityId { get; set; }
        public ReviewType ReviewType { get; set; }
    }
}
