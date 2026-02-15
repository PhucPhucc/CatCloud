using CatCloud.Core.Entities;
using CatCloud.Core.Interfaces;
using CatCloud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatCloud.Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly AppDbContext _context;
    public FolderRepository(AppDbContext context) => _context = context;

    public async Task AddFolderAsync(Folder folder)
    {
        _context.Folders.Add(folder);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Folder>> GetSubFoldersAsync(Guid? parentId, Guid userId)
    {
        // Lấy các folder có ParentId tương ứng và thuộc về User đó
        return await _context.Folders
            .Where(f => f.ParentId == parentId && f.OwnerId == userId) // Lưu ý: check userId ở đây là int hay Guid tùy project của bạn
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Folder?> GetFolderByIdAsync(Guid id)
    {
        return await _context.Folders.FindAsync(id);
    }
}