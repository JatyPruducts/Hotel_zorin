using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Hotel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Hotel.Infrastructure.Persistence.Repositories;

public class DepartureRepository : IDepartureRepository
{
    private readonly ToursDbContext _context;

    public DepartureRepository(ToursDbContext context)
    {
        _context = context;
    }

    public async Task<List<Departure>> GetAllAsync()
    {
        return await _context.Departures.ToListAsync();
    }

    public async Task<Departure?> GetByIdAsync(int id)
    {
        return await _context.Departures.FindAsync(id);
    }

    public async Task<int> CreateAsync(Departure departure)
    {
        if (departure == null) throw new ArgumentNullException(nameof(departure));

        _context.Departures.Add(departure);
        await _context.SaveChangesAsync();
        return departure.Id;
    }

    public async Task<bool> UpdateAsync(Departure departure)
    {
        if (departure == null) 
            throw new ArgumentNullException(nameof(departure));

        var existing = await _context.Departures.FindAsync(departure.Id);
        if (existing == null) return false;

        existing.Code = departure.Code;
        existing.Name = departure.Name;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Departures.FindAsync(id);
        if (existing == null) return false;

        _context.Departures.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}