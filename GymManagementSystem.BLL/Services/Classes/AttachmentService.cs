using GymManagementSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace GymManagementSystem.BLL.Services.Classes
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IWebHostEnvironment _env;
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        private readonly string[] _allowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png"
        };

        public AttachmentService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> UploadAsync(
            Stream fileStream,
            string folderName,
            string fileName,
            CancellationToken ct = default)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required.", nameof(fileName));

            if (fileStream.Length == 0)
                throw new Exception("The uploaded file is empty.");

            if (fileStream.Length > MaxFileSize)
                throw new Exception("The uploaded file exceeds the maximum allowed size (5 MB).");

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
                throw new Exception("Only .jpg, .jpeg and .png files are allowed.");

            var uploadsFolder = Path.Combine(_env.WebRootPath, folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, storedFileName);

            try
            {
                fileStream.Position = 0;

                await using var fs = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None);

                await fileStream.CopyToAsync(fs, ct);

                return storedFileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while uploading file: {ex.Message}", ex);
            }
        }

        public bool Delete(string folderName, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, folderName, fileName);
                if (!File.Exists(filePath)) return false;
                File.Delete(filePath);
                return true;

            }
            catch
            {
                return false;
            }
        }

        public (Stream stream, string contentType)? GetFile(string folderName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderName) || string.IsNullOrWhiteSpace(fileName))
                return null;

            var resolvedRoot = Path.GetFullPath(_env.WebRootPath);
            var filePath = Path.GetFullPath(Path.Combine(resolvedRoot, folderName, fileName));

            if (!filePath.StartsWith(resolvedRoot, StringComparison.OrdinalIgnoreCase))
                return null;

            if (!File.Exists(filePath))
                return null;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            return (stream, contentType);
        }

    }
}