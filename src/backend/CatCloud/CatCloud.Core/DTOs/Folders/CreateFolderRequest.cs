namespace CatCloud.Core.DTOs.Folders;

public class CreateFolderRequest
{
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; } = null;
}