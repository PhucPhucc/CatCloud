namespace CatCloud.Core.Entities;

public class FileMetadata
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OriginalFileName { get; set; } = string.Empty;

    public Guid OwnerId { get; set; } = Guid.Empty;
    
    public Guid? FolderId { get; set; } // Null = Nằm ở Root
    public Folder? Folder { get; set; }

    public string StoredFileName { get; set; } =
        string.Empty;

    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}