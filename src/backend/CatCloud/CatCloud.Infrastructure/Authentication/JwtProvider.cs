using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CatCloud.Core.Entities;
using CatCloud.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CatCloud.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // 1. Lấy Secret Key từ file cấu hình (appsettings.json)
        var secretKey = _configuration["Jwt:SecretKey"];
        if (secretKey == null) return "";
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 2. Định nghĩa các "Claims" (Dữ liệu đính kèm trong token)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // ID người dùng
            new Claim(ClaimTypes.Name, user.Username), // Tên
        };

        // 3. Tạo Token
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:Expire"])), 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }
}