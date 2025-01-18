using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeparturesController : ControllerBase
{
    private readonly IDepartureService _departureService;

    public DeparturesController(IDepartureService departureService)
    {
        // Проверка, что сервис не null
        _departureService = departureService 
            ?? throw new ArgumentNullException(nameof(departureService));
    }

    [HttpGet]
    public async Task<ActionResult<List<Departure>>> GetAll()
    {
        var departures = await _departureService.GetAllAsync();
        return Ok(departures);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Departure>> GetById(int id)
    {
        // Проверка, что id > 0
        if (id <= 0)
        {
            return BadRequest("Id must be greater than zero.");
        }

        var departure = await _departureService.GetByIdAsync(id);
        if (departure == null)
        {
            return NotFound();
        }

        return Ok(departure);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] Departure departure)
    {
        // Проверка на null
        if (departure == null)
        {
            return BadRequest("Departure cannot be null.");
        }

        // Дополнительные проверки полей
        if (string.IsNullOrWhiteSpace(departure.Code))
        {
            return BadRequest("Departure Code cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(departure.Name))
        {
            return BadRequest("Departure Name cannot be null or empty.");
        }

        var newId = await _departureService.CreateAsync(departure);

        // Возвращаем HTTP 201 (Created) с роутом на новый ресурс
        return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] Departure departure)
    {
        // Проверка на null
        if (departure == null)
        {
            return BadRequest("Departure cannot be null.");
        }

        // Проверяем, что Id в URL совпадает с Id в теле
        if (id != departure.Id)
        {
            return BadRequest("ID in URL and model do not match.");
        }

        // Дополнительные проверки полей
        if (string.IsNullOrWhiteSpace(departure.Code))
        {
            return BadRequest("Departure Code cannot be null or empty.");
        }
        if (string.IsNullOrWhiteSpace(departure.Name))
        {
            return BadRequest("Departure Name cannot be null or empty.");
        }

        var updated = await _departureService.UpdateAsync(departure);
        if (!updated)
        {
            return NotFound();
        }

        // При успешном обновлении принято возвращать NoContent (204)
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        // Проверка, что id > 0
        if (id <= 0)
        {
            return BadRequest("Id must be greater than zero.");
        }

        var deleted = await _departureService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}