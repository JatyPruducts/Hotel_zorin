namespace Hotel.Domain.Models.DTO;

public class LoginRequestDTO
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}