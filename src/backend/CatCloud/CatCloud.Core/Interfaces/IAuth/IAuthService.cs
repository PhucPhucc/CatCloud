using CatCloud.Core.DTOs.Auth;

namespace CatCloud.Core.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
    
    Task<string> RefreshTokenAsync(string refreshToken);
    
    Task LogoutAsync(string refreshToken);
}