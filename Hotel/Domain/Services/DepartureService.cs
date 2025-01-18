using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
namespace Hotel.Domain.Services;

public class DepartureService : IDepartureService
{
    private readonly IDepartureRepository _departureRepository;

    public DepartureService(IDepartureRepository departureRepository)
    {
        // Проверка, что репозиторий не null
        _departureRepository = departureRepository 
            ?? throw new ArgumentNullException(nameof(departureRepository));
    }

    public async Task<List<Departure>> GetAllAsync()
    {
        return await _departureRepository.GetAllAsync();
    }

    public async Task<Departure?> GetByIdAsync(int id)
    {
        // Проверка, что Id > 0
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        return await _departureRepository.GetByIdAsync(id);
    }

    public async Task<int> CreateAsync(Departure departure)
    {
        // Проверка, что сам объект не null
        if (departure == null)
        {
            throw new ArgumentNullException(nameof(departure), "Departure cannot be null.");
        }

        // Проверка обязательных полей
        if (string.IsNullOrWhiteSpace(departure.Code))
        {
            throw new ArgumentException("Departure Code cannot be null or empty.", nameof(departure.Code));
        }

        if (string.IsNullOrWhiteSpace(departure.Name))
        {
            throw new ArgumentException("Departure Name cannot be null or empty.", nameof(departure.Name));
        }

        return await _departureRepository.CreateAsync(departure);
    }

    public async Task<bool> UpdateAsync(Departure departure)
    {
        // Проверка, что сам объект не null
        if (departure == null)
        {
            throw new ArgumentNullException(nameof(departure), "Departure cannot be null.");
        }

        // Проверяем Id. Для обновления он должен быть > 0
        if (departure.Id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(departure.Id), "Id must be greater than zero when updating.");
        }

        // Проверка обязательных полей
        if (string.IsNullOrWhiteSpace(departure.Code))
        {
            throw new ArgumentException("Departure Code cannot be null or empty.", nameof(departure.Code));
        }

        if (string.IsNullOrWhiteSpace(departure.Name))
        {
            throw new ArgumentException("Departure Name cannot be null or empty.", nameof(departure.Name));
        }

        return await _departureRepository.UpdateAsync(departure);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // Проверка, что Id > 0
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        return await _departureRepository.DeleteAsync(id);
    }
}