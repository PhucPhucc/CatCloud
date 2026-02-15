namespace CatCloud.Core.Entities;

public class Folder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    
    // ParentId = null nghĩa là thư mục gốc (Root)
    public Guid? ParentId { get; set; } = null;
    
    public Guid OwnerId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties (Để EF Core hiểu quan hệ)
    public Folder? Parent { get; set; }
    public ICollection<Folder> SubFolders { get; set; } = new List<Folder>();
    public ICollection<FileMetadata> Files { get; set; } = new List<FileMetadata>();
}