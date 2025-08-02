using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Application.Services.Identity.Register.DTOs;
using Tripmate.Domain.Common.Response;

namespace Tripmate.Application.Services.Identity.Register
{
    public interface IRegisterHandler
    {
        Task<ApiResponse<string>> HandleRegisterAsync(RegisterDto registerDto);
    }
}
