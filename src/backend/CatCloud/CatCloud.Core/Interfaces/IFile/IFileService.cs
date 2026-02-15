using CatCloud.Core.DTOs.Files;
using CatCloud.Core.Entities;


namespace CatCloud.Core.Interfaces;

public interface IFileService
{
    Task<FileMetadata> UploadFileAsync(
        Stream fileStream, string fileName, string contentType, Guid userId, Guid? folderId);
    
    Task<FileDownloadDto> DownloadFileAsync(Guid fileId, Guid userId);
    
    Task DeleteFileAsync(Guid fileId, Guid userId);
}