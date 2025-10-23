using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Enums;

namespace Tripmate.Domain.Specification.FavoriteList
{
    public class FavoriteParameters
    {
        public FavoriteType? FavoriteType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
