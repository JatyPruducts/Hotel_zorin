using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;

namespace Hotel.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        // Проверим, нет ли уже
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            // Можно кидать кастомную ошибку:
            throw new InvalidOperationException("Username already taken.");
        }

        var hash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            PasswordHash = hash,
            CreatedAt = DateTime.Now
        };

        var newId = await _userRepository.CreateAsync(user);
        return newId;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return null;

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!valid) return null;

        return user;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _userRepository.GetByIdAsync(id);
    }
}