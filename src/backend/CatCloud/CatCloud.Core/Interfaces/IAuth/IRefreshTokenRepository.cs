using CatCloud.Core.Entities;

namespace CatCloud.Core.Interfaces.IAuth;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(RefreshToken token);
}