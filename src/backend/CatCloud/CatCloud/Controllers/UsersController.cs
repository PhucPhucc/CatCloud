using CatCloud.Core.Entities;
using CatCloud.Core.Interface;
using Microsoft.AspNetCore.Mvc;


namespace CatCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository userRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var users = await userRepository.GetAllUsersAsync();
        return Ok(users);
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