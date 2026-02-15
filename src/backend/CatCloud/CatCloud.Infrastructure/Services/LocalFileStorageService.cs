using CatCloud.Core.Interface;
using Microsoft.Extensions.Configuration;

namespace CatCloud.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["Storage:RootPath"]
                       ?? throw new Exception("Storage RootPath chưa được cấu hình");

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_storagePath, safeFileName);

        await using var outputStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None
        );

        await fileStream.CopyToAsync(outputStream);
        return filePath;
    }

    public Stream GetFileStream(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_storagePath, safeFileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File vật lý không tồn tại");

        return new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read
        );
    }

    public Task DeleteFileAsync(string fileName)
    {
        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(_storagePath, safeFileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("File vật lý không tồn tại");

        File.Delete(filePath);
        return Task.CompletedTask;
    }
}