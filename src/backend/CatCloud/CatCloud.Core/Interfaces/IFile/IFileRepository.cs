using CatCloud.Core.Entities;

namespace CatCloud.Core.Interfaces;

public interface IFileRepository
{
    Task AddFileAsync(FileMetadata file);
    Task<FileMetadata?> GetFileByIdAsync(Guid id);
    
    Task<List<FileMetadata>> GetFilesByFolderIdAsync(Guid? folderId, Guid userId);
    
    Task DeleteFileAsync(Guid id);
}