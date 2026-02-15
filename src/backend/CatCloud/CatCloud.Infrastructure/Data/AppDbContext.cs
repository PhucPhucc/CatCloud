using CatCloud.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatCloud.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public DbSet<FileMetadata> Files { get; set; }
    
    public DbSet<Folder> Folders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Cấu hình mối quan hệ đệ quy (Folder nằm trong Folder)
        modelBuilder.Entity<Folder>()
            .HasOne(f => f.Parent)
            .WithMany(f => f.SubFolders)
            .HasForeignKey(f => f.ParentId)
            .OnDelete(DeleteBehavior.Cascade); // Xóa cha -> Xóa con
    }
}