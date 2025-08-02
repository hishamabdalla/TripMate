using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Domain.Entities.Base
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       
    }
}
