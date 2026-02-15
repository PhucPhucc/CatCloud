using CatCloud.Core.Entities;
using CatCloud.Core.Interfaces.IAuth;
using CatCloud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatCloud.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();
    }
}