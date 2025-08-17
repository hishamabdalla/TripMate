using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Domain.Exceptions
{
    public class ImageValidationException: Exception
    {
        public ImageValidationException(string message) : base(message)
        {
        }
       
    }
}
