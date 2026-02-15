using CatCloud.Core.DTOs.Auth;
using CatCloud.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CatCloud.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return Ok("Đăng ký thành công");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            Response.Cookies.Append(
                "refresh_token",
                result.RefreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // bật khi dùng HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                }
            );
            return Ok(new { accessToken = result.AccessToken });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
                return Unauthorized();
            var newAccessToken = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(new { accessToken = newAccessToken });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            await _authService.LogoutAsync(refreshToken);
        }

        Response.Cookies.Delete("refresh_token");
        return Ok();
    }

}