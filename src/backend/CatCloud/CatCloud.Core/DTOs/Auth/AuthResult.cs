namespace CatCloud.Core.DTOs.Auth;

public class AuthResult
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}