using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;

namespace Hotel.Domain.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;

    public TourService(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public Task<List<Tour>> GetAllAsync()
    {
        return _tourRepository.GetAllAsync();
    }

    public Task<Tour?> GetByIdAsync(int id)
    {
        return _tourRepository.GetByIdAsync(id);
    }

    public Task<int> CreateAsync(Tour tour)
    {
        // Проверка, например, что price > 0, nights > 0 и т.д.
        return _tourRepository.CreateAsync(tour);
    }

    public Task<bool> UpdateAsync(Tour tour)
    {
        return _tourRepository.UpdateAsync(tour);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _tourRepository.DeleteAsync(id);
    }
}