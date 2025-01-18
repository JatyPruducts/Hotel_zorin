using Hotel.Domain.Models;

namespace Hotel.Domain.Interfaces;

public interface ITourService
{
    Task<List<Tour>> GetAllAsync();
    Task<Tour?> GetByIdAsync(int id);
    Task<int> CreateAsync(Tour tour);
    Task<bool> UpdateAsync(Tour tour);
    Task<bool> DeleteAsync(int id);
}