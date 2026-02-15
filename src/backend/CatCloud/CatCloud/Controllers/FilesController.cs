using System.Security.Claims;
using CatCloud.Core.Exceptions;
using CatCloud.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile file, [FromForm] Guid? folderId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Không tìm thấy thông tin user");
            if (file.Length == 0) return BadRequest("File rỗng");

            await using var stream = file.OpenReadStream();
            var result = await _fileService
                .UploadFileAsync(stream, file.FileName, file.ContentType, new Guid(userId), folderId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Lỗi server: " + ex.Message);
        }
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download([FromRoute] Guid id)
    {
        try
        {
            // 1. Lấy User Id từ Token (giống lúc upload)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();


            // 2. Gọi Service
            var fileDto = await _fileService.DownloadFileAsync(id, new Guid(userId));

            // 3. Trả về File Stream
            // File(Stream, ContentType, FileName) là helper method của Controller
            // Nó tự động xử lý việc buffer và range request (tua video)
            return File(fileDto.Stream, fileDto.ContentType, fileDto.FileName);
        }
        catch (FileNotFoundException)
        {
            return NotFound("File không tìm thấy trên ổ cứng");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); // 403 Forbidden
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message); // Hoặc 404 tùy logic
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            await _fileService.DeleteFileAsync(id, new Guid(userId));
            return NoContent();
        }
        catch (ResourceNotFoundException ex)
        {
            return NotFound(ex.Message); // Trả về 404
        }
        catch (ForbiddenAccessException ex)
        {
            return Forbid(ex.Message); // Trả về 403
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}