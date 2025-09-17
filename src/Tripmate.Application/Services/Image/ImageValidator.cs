using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Image
{
    public static class ImageValidator
    {
        private static readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long MaxSize = 2 * 1024 * 1024;
        public static bool BeValidImage(IFormFile file , bool isRequired)
        {
            if (file == null)
                return !isRequired;

            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_allowedExtensions.Contains(extension))
                return false;

            if (file.Length <= 0) 
                return false;

            if (file.Length > MaxSize) 
                return false;

            return true;
        }
        public static string GetErrorMessage(bool isRequired)
        {
            if (isRequired)
                return $"Image is required and must be .jpg, .jpeg, or .png with size <= {MaxSize / 1024 / 1024} MB.";

            return $"If provided, image must be .jpg, .jpeg, or .png and size <= {MaxSize / 1024 / 1024} MB.";
        }
    }
}
