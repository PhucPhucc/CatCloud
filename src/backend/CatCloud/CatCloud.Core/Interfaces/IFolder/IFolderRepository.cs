using CatCloud.Core.Entities;

namespace CatCloud.Core.Interfaces;

public interface IFolderRepository
{
    Task AddFolderAsync(Folder folder);
    
    Task<List<Folder>> GetSubFoldersAsync(Guid? parentId, Guid userId);
    
    Task<Folder?> GetFolderByIdAsync(Guid id);
}