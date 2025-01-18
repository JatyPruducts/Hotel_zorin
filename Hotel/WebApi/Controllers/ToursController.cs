using Hotel.Domain.Interfaces;
using Hotel.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.WebApi.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly ILogger<ToursController> _logger;

        public ToursController(ITourService tourService, ILogger<ToursController> logger)
        {
            _tourService = tourService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Tour>>> GetAll()
        {
            try
            {
                var tours = await _tourService.GetAllAsync();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAll Tours");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tour>> GetById(int id)
        {
            try
            {
                var tour = await _tourService.GetByIdAsync(id);
                if (tour == null)
                    return NotFound("Tour not found.");

                return Ok(tour);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetById Tour");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] Tour tour)
        {
            try
            {
                var newId = await _tourService.CreateAsync(tour);
                return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create Tour");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Tour tour)
        {
            try
            {
                if (id != tour.Id)
                    return BadRequest("URL id and model id do not match.");

                var updated = await _tourService.UpdateAsync(tour);
                if (!updated)
                    return NotFound("Tour not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Update Tour");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _tourService.DeleteAsync(id);
                if (!deleted)
                    return NotFound("Tour not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Delete Tour");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
