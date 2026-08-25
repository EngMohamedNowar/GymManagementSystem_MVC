using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<string?> UploadAsync(Stream fileStream, string folderPath,string fileName,CancellationToken ct = default);
        bool Delete(string folderName, string fileName);
        (Stream stream,string contentType)? GetFile(string folderName, string fileName);
    }
}
