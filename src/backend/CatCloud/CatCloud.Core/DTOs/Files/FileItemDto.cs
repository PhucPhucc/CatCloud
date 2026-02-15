namespace CatCloud.Core.DTOs.Files;

public class FileItemDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; }
    public string ContentType { get; set; }
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
}