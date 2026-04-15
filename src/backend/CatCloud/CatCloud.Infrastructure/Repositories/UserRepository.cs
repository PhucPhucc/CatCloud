using CatCloud.Core.Entities;
using CatCloud.Core.Interface;
using CatCloud.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CatCloud.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);

    }

    public async Task<string?> GetMe(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.Username;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public Task DeleteUserAsync(Guid id)
    {
        _context.Users.Remove(_context.Users.Find(id) ?? throw new InvalidOperationException());
        return _context.SaveChangesAsync();
    }
}