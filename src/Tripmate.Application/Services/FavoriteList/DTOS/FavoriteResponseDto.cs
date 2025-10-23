using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.FavoriteList.DTOS
{
    public class FavoriteResponseDto
    {
        public int Id { get; set; }
        public FavoriteType FavoriteType { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
