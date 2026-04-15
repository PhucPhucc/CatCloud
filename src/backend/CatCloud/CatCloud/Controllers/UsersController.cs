using System.Security.Claims;
using CatCloud.Core.Entities;
using CatCloud.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CatCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IUserRepository userRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userRepository.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized("Không tìm thấy thông tin user");
            var userName = await userRepository.GetMe(new Guid(userId));
            return Ok(userName);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        await userRepository.AddUserAsync(user);
        return Ok(user);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromBody] Guid id)
    {
        await userRepository.DeleteUserAsync(id);
        return Ok();
    }
}