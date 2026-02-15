using CatCloud.Core.DTOs.Files;
using CatCloud.Core.DTOs.Folders;
using CatCloud.Core.Entities;
using CatCloud.Core.Exceptions;
using CatCloud.Core.Interface.IFolder;
using CatCloud.Core.Interfaces;

namespace Application.Services;

public class FolderService : IFolderService
{
    private readonly IFolderRepository _folderRepository;
    private readonly IFileRepository _fileRepository;

    public FolderService(IFolderRepository folderRepository, IFileRepository fileRepository)
    {
        _folderRepository = folderRepository;
        _fileRepository = fileRepository;
    }

    public async Task<FolderContentDto> GetFolderContentAsync(Guid? folderId, Guid userId)
    {
        var result = new FolderContentDto();

        // TRƯỜNG HỢP 1: Xem nội dung một thư mục cụ thể
        if (folderId.HasValue)
        {
            // Check folder có tồn tại và đúng chủ không
            var currentFolder = await _folderRepository.GetFolderByIdAsync(folderId.Value);

            if (currentFolder == null)
                throw new ResourceNotFoundException("Thư mục không tồn tại");

            // (Giả sử OwnerId là int, nếu project bạn là Guid thì đổi kiểu int -> Guid)
            if (currentFolder.OwnerId != userId)
                throw new ForbiddenAccessException("Bạn không có quyền xem thư mục này");

            result.CurrentFolderId = currentFolder.Id;
            result.CurrentFolderName = currentFolder.Name;
            result.ParentId = currentFolder.ParentId;
        }
        // TRƯỜNG HỢP 2: Xem Root (folderId == null) -> Giữ nguyên mặc định (Name="Root", ParentId=null)

        // 1. Lấy danh sách Folder con
        var subFolders = await _folderRepository.GetSubFoldersAsync(folderId, userId);
        result.SubFolders = subFolders.Select(f => new FolderItemDto
        {
            Id = f.Id,
            Name = f.Name,
            CreatedAt = f.CreatedAt
        }).ToList();

        // 2. Lấy danh sách File con
        // (Cần inject thêm IFileRepository vào constructor của FolderService nhé)
        var files = await _fileRepository.GetFilesByFolderIdAsync(folderId, userId);
        result.Files = files.Select(f => new FileItemDto()
        {
            Id = f.Id,
            OriginalFileName = f.OriginalFileName,
            ContentType = f.ContentType,
            Size = f.Size,
            CreatedAt = f.UploadedAt
        }).ToList();

        return result;
    }

    public async Task<FolderContentDto> CreateFolderAsync(CreateFolderRequest request, Guid userId)
    {
        // 1. Nếu user muốn tạo folder con, phải check xem folder cha có tồn tại và thuộc về user đó không
        if (request.ParentId.HasValue)
        {
            var parentFolder = await _folderRepository.GetFolderByIdAsync(request.ParentId.Value);
            if (parentFolder == null)
                throw new ResourceNotFoundException("Thư mục cha không tồn tại");

            if (parentFolder.OwnerId != userId)
                throw new ForbiddenAccessException("Bạn không được phép tạo folder ở đây");
        }

        // 2. Tạo Entity
        var folder = new Folder
        {
            Name = request.Name,
            ParentId = request.ParentId,
            OwnerId = userId
        };

        // 3. Lưu DB
        await _folderRepository.AddFolderAsync(folder);
        return new FolderContentDto { CurrentFolderId = folder.Id, CurrentFolderName = folder.Name };
    }
}