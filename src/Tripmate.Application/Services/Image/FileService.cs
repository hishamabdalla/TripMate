using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Tripmate.Application.Services.Image.ImagesFolders;
using Tripmate.Domain.Exceptions;

namespace Tripmate.Application.Services.Image
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileService> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileService(IWebHostEnvironment webHostEnvironment, ILogger<FileService> logger)
        {
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string? folderName)
        {
            _logger.LogInformation("Starting image upload for folder: {Folder}, file: {FileName}, size: {FileSize} KB", 
                folderName, imageFile.FileName, imageFile.Length/1024);

            if (imageFile == null)
            {
                _logger.LogWarning("Image upload failed: Image file cannot be null.");
                throw new ImageValidationException("Image file cannot be null.");
            }

            if (imageFile.Length == 0)
            {
                _logger.LogWarning("Image upload failed: Image file cannot be empty.");
                throw new ImageValidationException("Image file cannot be empty.");
            }

            if (!_allowedExtensions.Contains(Path.GetExtension(imageFile.FileName).ToLower()))
            {
                _logger.LogWarning("Image upload failed: Invalid file type. Allowed types are: {AllowedExtensions}", 
                    string.Join(", ", _allowedExtensions));
                throw new ImageValidationException($"Invalid file type. Allowed types are: {string.Join(", ", _allowedExtensions)}.");
            }

            if (imageFile.Length > _maxFileSize)
            {
                _logger.LogWarning("Image upload failed: Image file size {FileSize} exceeds maximum allowed size {MaxSize}", 
                    imageFile.Length, _maxFileSize);
                throw new ImageValidationException($"Image file size cannot exceed {_maxFileSize / 1024 / 1024} MB.");
            }

            try
            {
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                _logger.LogDebug("Generated unique filename: {FileName}", imageName);

                var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", folderName??FoldersNames.Defualt);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    _logger.LogInformation("Created directory: {Directory}", folderPath);
                }

                var filePath = Path.Combine(folderPath, imageName);
                _logger.LogDebug("Full file path: {FilePath}", filePath);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(fileStream);

                _logger.LogInformation("Successfully uploaded image: {ImageUrl}, original filename: {OriginalFileName}", 
                    filePath, imageFile.FileName);

                return imageName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image for folder: {Folder}, filename: {FileName}", 
                    folderName, imageFile.FileName);
                throw new ImageValidationException("Failed to upload image. Please try again.");
            }
        }

        public void DeleteImage(string imageUrl, string? folderName)
        {
            _logger.LogInformation("Attempting to delete image: {ImageUrl} from folder: {Folder}", imageUrl, folderName);

            if (string.IsNullOrEmpty(imageUrl))
            {
                _logger.LogWarning("Delete image failed: Image URL cannot be null or empty.");
                throw new ImageValidationException("Image URL cannot be null or empty.");
            }

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", folderName ?? FoldersNames.Defualt, imageUrl);
            _logger.LogDebug("Full deletion path: {FullPath}", filePath);

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("Image file not found for deletion: {FullPath}", filePath);
                throw new FileNotFoundException("Image file not found.");
            }

            File.Delete(filePath);
            _logger.LogInformation("Successfully deleted image: {ImageUrl}", imageUrl);
        }

      
    }
}
