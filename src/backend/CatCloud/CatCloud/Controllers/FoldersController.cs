using System.Security.Claims;
using CatCloud.Core.DTOs.Folders;
using CatCloud.Core.Interface.IFolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FoldersController : ControllerBase
{
    private readonly IFolderService _folderService;

    public FoldersController(IFolderService folderService)
    {
        _folderService = folderService;
    }

    [HttpGet("{id:guid?}")]
    public async Task<IActionResult> GetAllChildrenFolders(Guid? id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var content = await _folderService.GetFolderContentAsync(id, new Guid(userId));
            return Ok(content);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var folder = await _folderService.CreateFolderAsync(request, new Guid(userId));
            return Ok(folder);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}