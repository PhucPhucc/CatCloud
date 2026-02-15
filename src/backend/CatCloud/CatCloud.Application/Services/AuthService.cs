using CatCloud.Core.DTOs.Auth;
using CatCloud.Core.Entities;
using CatCloud.Core.Interface;
using CatCloud.Core.Interfaces;
using CatCloud.Core.Interfaces.IAuth;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtProvider jwtProvider,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        // 1. Check user tồn tại (bạn nên thêm hàm GetByUsername vào IUserRepository nhé)
        var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username);
        if (existingUser != null) throw new Exception("Người dùng đã tồn tại");

        // 2. Hash Password (QUAN TRỌNG: Không bao giờ lưu pass thô)
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 3. Tạo User Entity
        var user = new User
        {
            Username = request.Username,
            PasswordHash = passwordHash
        };

        // 4. Lưu vào DB
        await _userRepository.AddUserAsync(user);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        // 1. Tìm user trong DB
        var user = await _userRepository.GetUserByUsernameAsync(request.Username);

        if (user == null)
        {
            throw new Exception("Sai tài khoản hoặc mật khẩu");
        }

        // 2. Verify Password (So sánh pass nhập vào và hash trong DB)
        var isValidPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!isValidPassword)
        {
            throw new Exception("Sai tài khoản hoặc mật khẩu");
        }

        // 3. Nếu đúng -> Tạo Token
        var accessToken = _jwtProvider.GenerateToken(user);

        var refreshTokenValue = _jwtProvider.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);


        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue
        };
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new Exception("RefreshToken khong ton tai hoac da het han");

        var user = await _userRepository.GetUserByIdAsync(storedToken.UserId);

        return user == null 
            ? throw new Exception("User khong ton tai") 
            : _jwtProvider.GenerateToken(user);
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken != null)
            await _refreshTokenRepository.RevokeAsync(storedToken);
    }
}