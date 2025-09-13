using CarAPI.Data;
using CarAPI.Models;
using CarAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarsApiController : ControllerBase
    {
        private readonly ICarService _carService;

        public CarsApiController(ICarService carService)
        {
            _carService = carService;
        }

        [HttpGet("api/cars")]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _carService.GetAllCarsAsync();
            return Ok(cars);
        }

        [HttpGet("api/persons/{personId}/cars")]
        public async Task<IActionResult> GetCarsForPerson(int personId)
        {
            var cars = await _carService.GetCarsByPersonIdAsync(personId);
            if (cars == null)
                return NotFound(new { error = "Person not found." });

            return Ok(cars);
        }

        [HttpPost("api/cars")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateFromJson([FromBody] Car car)
        {
            try
            {
                var createdCar = await _carService.CreateCarAsync(car);
                return CreatedAtAction(nameof(GetCarById), new { id = createdCar.Id }, createdCar);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        //private bool CarExists(int id)
        //{
        //    return await _carService.CarExistsAsync(id);
        //}

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var carDto = await _carService.GetCarDtoByIdAsync(id);
            if (carDto == null)
                return NotFound();

            return Ok(carDto);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Car updatedCar)
        {
            if (id != updatedCar.Id)
                return BadRequest(new { error = "Car ID mismatch." });

            var success = await _carService.UpdateCarAsync(updatedCar);
            if (!success)
                return NotFound(new { error = $"Car with ID {id} not found." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var success = await _carService.DeleteCarAsync(id);
            if (!success)
                return NotFound(new { error = $"Car with ID {id} not found." });

            return NoContent();
        }

    }
}
