using Hotel.Domain.Models;

namespace Hotel.Domain.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Создать пользователя (регистрация). Возвращает Id нового пользователя или -1 (если ошибка).
    /// </summary>
    Task<int> RegisterAsync(string username, string password);

    /// <summary>
    /// Проверить логин + пароль. Возвращает User при успехе или null, если неверно.
    /// </summary>
    Task<User?> LoginAsync(string username, string password);

    /// <summary>
    /// Получить пользователя по Id (опционально).
    /// </summary>
    Task<User?> GetByIdAsync(int id);
}