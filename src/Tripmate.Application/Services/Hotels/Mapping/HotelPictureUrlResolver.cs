using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Hotels.DTOS;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Application.Services.Image.PictureResolver;
using Tripmate.Domain.Entities.Models;

namespace Tripmate.Application.Services.Hotels.Mapping
{
    public class HotelPictureUrlResolver : GenericPictureUrlResolver<Hotel, ReadHotelDto>
    {
        public HotelPictureUrlResolver(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor, FoldersNames.Hotels)
        {
        }
    }
}
