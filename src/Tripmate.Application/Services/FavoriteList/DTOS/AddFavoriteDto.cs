using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Enums;

namespace Tripmate.Application.Services.FavoriteList.DTOS
{
    public class AddFavoriteDto
    {
        public FavoriteType FavoriteType { get; set; }
        public int EntityId { get; set; }
    }
}
