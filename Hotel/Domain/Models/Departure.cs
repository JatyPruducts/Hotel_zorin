namespace Hotel.Domain.Models;

public class Departure
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}