using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Hotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastructure.Persistence.Repositories;

public class TourRepository : ITourRepository
{
    private readonly ToursDbContext _context;

    public TourRepository(ToursDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tour>> GetAllAsync()
    {
        return await _context.Tours.ToListAsync();
    }

    public async Task<Tour?> GetByIdAsync(int id)
    {
        return await _context.Tours.FindAsync(id);
    }

    public async Task<int> CreateAsync(Tour tour)
    {
        if (tour == null) throw new ArgumentNullException(nameof(tour));

        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();  // EF сгенерирует Id
        return tour.Id;
    }

    public async Task<bool> UpdateAsync(Tour tour)
    {
        if (tour == null) 
            throw new ArgumentNullException(nameof(tour));

        // Проверяем, есть ли в БД
        var existing = await _context.Tours.FindAsync(tour.Id);
        if (existing == null) return false;

        // Обновляем нужные поля
        existing.Title = tour.Title;
        existing.Description = tour.Description;
        existing.DepartureId = tour.DepartureId;
        existing.Picture = tour.Picture;
        existing.Price = tour.Price;
        existing.Stars = tour.Stars;
        existing.Country = tour.Country;
        existing.Nights = tour.Nights;
        existing.Date = tour.Date;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Tours.FindAsync(id);
        if (existing == null) return false;

        _context.Tours.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}