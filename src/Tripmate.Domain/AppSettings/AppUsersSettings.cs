using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Domain.AppSettings
{
    public class AppUsersSettings
    {
        public List<AppUser> AppUsers { get; set; }
    }

    public class AppUser
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Country { get; set; }

    }
}
