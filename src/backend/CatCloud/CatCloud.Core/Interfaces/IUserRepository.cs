using CatCloud.Core.Entities;

namespace CatCloud.Core.Interface;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    
    Task<User?> GetUserByIdAsync(Guid userId);
    
    Task<User?> GetUserByUsernameAsync(string username);
    
    Task AddUserAsync(User user);
    
    Task DeleteUserAsync(Guid id);
}