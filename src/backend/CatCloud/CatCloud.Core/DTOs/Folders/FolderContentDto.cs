using CatCloud.Core.DTOs.Files;
using CatCloud.Core.Entities;

namespace CatCloud.Core.DTOs.Folders;

public class FolderContentDto
{
    public Guid? CurrentFolderId { get; set; }
    public string CurrentFolderName { get; set; } = "Root"; // Mặc định là Root
    public Guid? ParentId { get; set; } // Để làm nút "Back"

    // Danh sách folder con
    public List<FolderItemDto> SubFolders { get; set; } = new();

    // Danh sách file
    public List<FileItemDto> Files { get; set; } = new();
}