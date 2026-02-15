namespace CatCloud.Core.DTOs.Files;

public class FileDownloadDto
{
    public Stream Stream { get; set; }
    public string ContentType { get; set; }
    public string FileName { get; set; }
}