namespace Hotel.Domain.Models;

public class Tour
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int DepartureId { get; set; }
    public string Picture { get; set; } = null!;
    public int Price { get; set; }
    public int Stars { get; set; }
    public string Country { get; set; } = null!;
    public int Nights { get; set; }
    public string Date { get; set; } = null!;
}