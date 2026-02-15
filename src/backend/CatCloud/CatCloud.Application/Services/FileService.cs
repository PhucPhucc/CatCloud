using CatCloud.Core.DTOs.Files;
using CatCloud.Core.Entities;
using CatCloud.Core.Exceptions;
using CatCloud.Core.Interface;
using CatCloud.Core.Interfaces;
using CatCloud.Core.Interfaces.IFile;


namespace Application.Services;

public class FileService : IFileService
{
    // private readonly IFileStorageService _storageService;
    private readonly IUserFileStorageService _storageService;
    private readonly IFileRepository _fileRepository;

    public FileService(IUserFileStorageService storageService, IFileRepository fileRepository)
    {
        _storageService = storageService;
        _fileRepository = fileRepository;
    }

    public async Task<FileMetadata> UploadFileAsync(Stream fileStream, string fileName, string contentType, Guid userId, Guid? folderId)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File rỗng!");
        }


        var storedFileName = await _storageService.SaveFileAsync(userId, fileStream, Path.GetExtension(fileName));

        var fileMetadata = new FileMetadata
        {
            OwnerId = userId,
            FolderId = folderId,
            OriginalFileName = fileName,
            StoredFileName = storedFileName,
            ContentType = contentType,
            Size = fileStream.Length,
        };


        await _fileRepository.AddFileAsync(fileMetadata);

        return fileMetadata;
    }

    public async Task<FileDownloadDto> DownloadFileAsync(Guid fileId, Guid userId)
    {
        var fileMetadata = await GetFileAndValidateOwnerAsync(fileId, userId);
        // 3. Lấy Stream từ ổ cứng
        // Lưu ý: fileMetadata.StoredFileName là tên file mã hóa trên đĩa (guid)
        var stream = _storageService.GetFileStream(userId, fileMetadata.StoredFileName);

        // 4. Trả về DTO
        return new FileDownloadDto
        {
            Stream = stream,
            ContentType = fileMetadata.ContentType,
            FileName = fileMetadata.OriginalFileName // Trả về tên gốc cho user vui
        };
    }

    public async Task DeleteFileAsync(Guid fileId, Guid userId)
    {
        var fileMetadata = await GetFileAndValidateOwnerAsync(fileId, userId);
        
        await _fileRepository.DeleteFileAsync(fileId);
        await _storageService.DeleteFileAsync(userId, fileMetadata.StoredFileName);
    }
    
    // Hàm này trả về FileMetadata nếu hợp lệ, nếu không thì ném lỗi
    private async Task<FileMetadata> GetFileAndValidateOwnerAsync(Guid fileId, Guid userId)
    {
        var fileMetadata = await _fileRepository.GetFileByIdAsync(fileId);

        // 1. Kiểm tra tồn tại
        if (fileMetadata == null)
        {
            throw new ResourceNotFoundException($"File với ID {fileId} không tồn tại.");
        }

        // 2. Kiểm tra quyền sở hữu
        if (fileMetadata.OwnerId != userId)
        {
            throw new ForbiddenAccessException("Bạn không có quyền tác động lên file này.");
        }


        return fileMetadata;
    }
}