using CatCloud.Core.Interface;
using CatCloud.Core.Interfaces.IFile;
using Microsoft.Extensions.Configuration;

namespace CatCloud.Infrastructure.Services;

public class UserFileStorageService : IUserFileStorageService
{
    private readonly string _rootPath;

    public UserFileStorageService(IConfiguration configuration)
    {
        _rootPath = configuration["Storage:RootPath"]
            ?? throw new Exception("Storage:RootPath chưa được cấu hình");

        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveFileAsync(
        Guid userId,
        Stream fileStream,
        string fileExtension
    )
    {
        var fileId = $"f_{Guid.NewGuid():N}";
        var userFilesDir = GetUserFilesDir(userId);

        Directory.CreateDirectory(userFilesDir);

        var filePath = Path.Combine(
            userFilesDir,
            $"{fileId}{NormalizeExtension(fileExtension)}"
        );

        await using var output = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None
        );

        await fileStream.CopyToAsync(output);
        return fileId;
    }

    public Stream GetFileStream(
        Guid userId,
        string fileId,
        bool fromTrash = false
    )
    {
        var dir = fromTrash
            ? GetUserTrashDir(userId)
            : GetUserFilesDir(userId);

        var filePath = FindFileById(dir, fileId);

        if (filePath == null)
            throw new FileNotFoundException("File không tồn tại");

        return new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read
        );
    }

    public Task DeleteFileAsync(
        Guid userId,
        string fileId
    )
    {
        var sourceDir = GetUserFilesDir(userId);
        var trashDir = GetUserTrashDir(userId);

        Directory.CreateDirectory(trashDir);

        var sourcePath = FindFileById(sourceDir, fileId);
        if (sourcePath == null)
            throw new FileNotFoundException("File không tồn tại");

        var fileName = Path.GetFileName(sourcePath);
        var targetPath = Path.Combine(trashDir, fileName);

        File.Move(sourcePath, targetPath, overwrite: true);
        return Task.CompletedTask;
    }

    public Task RestoreFileAsync(
        Guid userId,
        string fileId
    )
    {
        var trashDir = GetUserTrashDir(userId);
        var filesDir = GetUserFilesDir(userId);

        Directory.CreateDirectory(filesDir);

        var sourcePath = FindFileById(trashDir, fileId);
        if (sourcePath == null)
            throw new FileNotFoundException("File không tồn tại trong trash");

        var fileName = Path.GetFileName(sourcePath);
        var targetPath = Path.Combine(filesDir, fileName);

        File.Move(sourcePath, targetPath, overwrite: true);
        return Task.CompletedTask;
    }

    /* =========================
       PRIVATE HELPERS
       ========================= */

    private string GetUserRoot(Guid userId)
        => Path.Combine(_rootPath, "users", userId.ToString());

    private string GetUserFilesDir(Guid userId)
        => Path.Combine(GetUserRoot(userId), "files");

    private string GetUserTrashDir(Guid userId)
        => Path.Combine(GetUserRoot(userId), "trash");

    private static string NormalizeExtension(string ext)
    {
        if (string.IsNullOrWhiteSpace(ext))
            return string.Empty;

        return ext.StartsWith('.') ? ext : $".{ext}";
    }

    private static string? FindFileById(string directory, string fileId)
    {
        if (!Directory.Exists(directory))
            return null;

        return Directory
            .EnumerateFiles(directory)
            .FirstOrDefault(f =>
                Path.GetFileNameWithoutExtension(f) == fileId);
    }
}
