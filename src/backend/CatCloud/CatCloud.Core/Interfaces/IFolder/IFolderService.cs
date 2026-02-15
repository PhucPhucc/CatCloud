using CatCloud.Core.DTOs.Folders;

namespace CatCloud.Core.Interface.IFolder;

public interface IFolderService
{
    Task<FolderContentDto> GetFolderContentAsync(Guid? folderId, Guid userId);    
    Task<FolderContentDto> CreateFolderAsync(CreateFolderRequest request, Guid userId);
}