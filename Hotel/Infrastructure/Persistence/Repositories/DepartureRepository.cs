using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Npgsql;

namespace Hotel.Infrastructure.Persistence.Repositories;

public class DepartureRepository : IDepartureRepository
{
    private readonly string _connectionString;

    public DepartureRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task<List<Departure>> GetAllAsync()
    {
        var result = new List<Departure>();

        // Подключение к базе данных
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // Команда для выбора всех записей
        const string sql = "SELECT id, code, name FROM departures";
        using var cmd = new NpgsqlCommand(sql, conn);

        // Чтение результатов
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var id = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
            var code = !reader.IsDBNull(1) ? reader.GetString(1) : null;
            var name = !reader.IsDBNull(2) ? reader.GetString(2) : null;

            result.Add(new Departure
            {
                Id = id,
                Code = code,
                Name = name
            });
        }

        return result;
    }

    public async Task<Departure?> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        }

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = "SELECT id, code, name FROM departures WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var dbId = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
            var code = !reader.IsDBNull(1) ? reader.GetString(1) : null;
            var name = !reader.IsDBNull(2) ? reader.GetString(2) : null;

            return new Departure
            {
                Id = dbId,
                Code = code,
                Name = name
            };
        }

        return null;
    }

    public async Task<int> CreateAsync(Departure departure)
    {
        if (departure == null) 
            throw new ArgumentNullException(nameof(departure));
        
        if (string.IsNullOrWhiteSpace(departure.Code))
            throw new ArgumentException("Departure Code cannot be null or empty.", nameof(departure.Code));

        if (string.IsNullOrWhiteSpace(departure.Name))
            throw new ArgumentException("Departure Name cannot be null or empty.", nameof(departure.Name));

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            INSERT INTO departures (code, name)
            VALUES (@code, @name)
            RETURNING id;
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("code", departure.Code);
        cmd.Parameters.AddWithValue("name", departure.Name);

        var result = await cmd.ExecuteScalarAsync();
        // Возвращаем скастованный Id, или 0, если почему-то пришло что-то иное
        return result is int newId ? newId : 0;
    }

    public async Task<bool> UpdateAsync(Departure departure)
    {
        if (departure == null) 
            throw new ArgumentNullException(nameof(departure));

        if (departure.Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(departure.Id), "Id must be greater than zero when updating.");

        if (string.IsNullOrWhiteSpace(departure.Code))
            throw new ArgumentException("Departure Code cannot be null or empty.", nameof(departure.Code));
        
        if (string.IsNullOrWhiteSpace(departure.Name))
            throw new ArgumentException("Departure Name cannot be null or empty.", nameof(departure.Name));

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            UPDATE departures 
            SET code = @code, name = @name
            WHERE id = @id
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("code", departure.Code);
        cmd.Parameters.AddWithValue("name", departure.Name);
        cmd.Parameters.AddWithValue("id", departure.Id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = "DELETE FROM departures WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}