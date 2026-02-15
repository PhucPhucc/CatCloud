namespace CatCloud.Core.Interface;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName);
    
    Stream GetFileStream(string fileName);
    
    Task DeleteFileAsync(string fileName);
}