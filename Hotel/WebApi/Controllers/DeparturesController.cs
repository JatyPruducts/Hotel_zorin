using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.WebApi.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class DeparturesController : ControllerBase
    {
        private readonly IDepartureService _departureService;
        private readonly ILogger<DeparturesController> _logger;

        public DeparturesController(IDepartureService departureService, ILogger<DeparturesController> logger)
        {
            _departureService = departureService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Departure>>> GetAll()
        {
            try
            {
                var departures = await _departureService.GetAllAsync();
                return Ok(departures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll Departures");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Departure>> GetById(int id)
        {
            try
            {
                var departure = await _departureService.GetByIdAsync(id);
                if (departure == null)
                    return NotFound("Departure not found.");

                return Ok(departure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById Departures");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] Departure departure)
        {
            try
            {
                // Можно добавить проверки departure.Code и т.д.
                var newId = await _departureService.CreateAsync(departure);
                return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create Departure");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Departure departure)
        {
            try
            {
                if (id != departure.Id)
                    return BadRequest("URL id and model id do not match.");

                var updated = await _departureService.UpdateAsync(departure);
                if (!updated)
                    return NotFound("Departure not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update Departure");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _departureService.DeleteAsync(id);
                if (!deleted)
                    return NotFound("Departure not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete Departure");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }