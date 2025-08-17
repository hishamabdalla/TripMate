using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tripmate.Domain.Exceptions;

namespace Tripmate.Application.Services.Image
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string? folderName)
        {
            if (imageFile == null)
            {
                throw new ImageValidationException("Image file cannot be null.");
            }

            if (imageFile.Length == 0)
            {
                throw new ImageValidationException("Image file cannot be empty.");
            }
            if(!_allowedExtensions.Contains(Path.GetExtension(imageFile.FileName).ToLower()))
            {
                throw new ImageValidationException($"Invalid file type. Allowed types are: {string.Join(", ", _allowedExtensions)}.");
            }
            if (imageFile.Length > _maxFileSize)
            {
                throw new ImageValidationException($"Image file size cannot exceed {_maxFileSize / 1024 / 1024} MB.");
            }
          


            // Generate a unique file name to avoid conflicts
            var imageName =$"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";

         

            var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", folderName);

            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var filePath = Path.Combine(FolderPath, imageName);

            using var fileStream = new FileStream(filePath, FileMode.Create);

            await imageFile.CopyToAsync(fileStream);

           return imageName;


        }
        public void DeleteImage(string imageUrl,string? folderName )
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ImageValidationException( "Image URL cannot be null or empty.");
            }
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images",folderName, imageUrl);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Image file not found.");

            }
            File.Delete(filePath);

        }
    }
}
