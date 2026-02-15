using CatCloud.Core.Entities;

namespace CatCloud.Core.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
    
    string GenerateRefreshToken();
}