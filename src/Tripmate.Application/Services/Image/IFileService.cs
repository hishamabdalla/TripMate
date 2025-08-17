using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tripmate.Application.Services.Image
{
    public interface IFileService
    {
        /// <summary>
        /// Uploads an image file and returns the URL of the uploaded image.
        /// </summary>
        /// <param name="image">The image file to upload. Must not be null and should be a valid image format.</param>
        /// the uploaded image.</returns>
        Task<string> UploadImageAsync(IFormFile image, string? folderName = "default");

        void DeleteImage(string imageUrl, string? folderName = "default");

    }
}
