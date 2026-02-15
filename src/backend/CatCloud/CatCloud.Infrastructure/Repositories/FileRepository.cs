using CatCloud.Core.Entities;
using CatCloud.Core.Interfaces;
using CatCloud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatCloud.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _context;

    public FileRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddFileAsync(FileMetadata file)
    {
        _context.Files.Add(file);
        await _context.SaveChangesAsync();
    }

    public async Task<FileMetadata?> GetFileByIdAsync(Guid id)
    {
        return await _context.Files.FindAsync(id);
    }

    public async Task<List<FileMetadata>> GetFilesByFolderIdAsync(Guid? folderId, Guid userId)
    {
        return await _context.Files
            .Where(f => f.FolderId == folderId && f.OwnerId == userId) // OwnerId int hay Guid? Check lại Entity User của bạn
            .OrderByDescending(f => f.UploadedAt)
            .ToListAsync();
    }
    
    public async Task DeleteFileAsync(Guid id)
    {
        var file = await _context.Files.FindAsync(id);
        if (file != null) _context.Files.Remove(file);
        await _context.SaveChangesAsync();
    }
}