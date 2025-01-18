using Hotel.Domain.Models;
namespace Hotel.Domain.Interfaces;

public interface IDepartureService
{
    Task<List<Departure>> GetAllAsync();
    Task<Departure?> GetByIdAsync(int id);
    Task<int> CreateAsync(Departure departure);
    Task<bool> UpdateAsync(Departure departure);
    Task<bool> DeleteAsync(int id);
}