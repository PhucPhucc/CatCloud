namespace CatCloud.Core.Interfaces.IFile;

public interface IUserFileStorageService
{
    Task<string> SaveFileAsync(
        Guid userId,
        Stream fileStream,
        string fileExtension
    );

    Stream GetFileStream(
        Guid userId,
        string fileId,
        bool fromTrash = false
    );

    Task DeleteFileAsync(
        Guid userId,
        string fileId
    );

    Task RestoreFileAsync(
        Guid userId,
        string fileId
    );
}