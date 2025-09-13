using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarAPI.Data;
using CarAPI.Models;

namespace CarAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("api/cars")]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _context.Cars
                .Include(c => c.Person)
                .Select(static c => new CarDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    Make = c.Make,
                    Year = c.Year,
                    Color = c.Color,
                    Price = (double)c.Price,
                    PersonId = c.PersonId ?? 0,
                    PersonName = c.Person != null ? c.Person.Name : null
                })
                .ToListAsync();

            return Ok(cars);
        }

        [HttpGet]
        [Route("api/persons/{personId}/cars")]
        public async Task<IActionResult> GetCarsForPerson(int personId)
        {
            var personExists = await _context.People.AnyAsync(p => p.Id == personId);
            if (!personExists)
            {
                return NotFound(new { error = "Person not found." });
            }

            var cars = await _context.Cars
                .Where(c => c.PersonId == personId)
                .ToListAsync();

            return Ok(cars);
        }

        [HttpGet]
        [Route("api/persons")]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _context.People
                .Include(p => p.Cars)
                .Select(p => new PersonDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    Cars = p.Cars.Select(c => new CarDto
                    {
                        Id = c.Id,
                        Model = c.Model,
                        Make = c.Make,
                        Year = c.Year
                    }).ToList()
                })
                .ToListAsync();

            return Ok(persons);
        }


        [HttpPost]
        [Route("api/persons")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            if (person == null || string.IsNullOrWhiteSpace(person.Name) || string.IsNullOrWhiteSpace(person.Email))
            {
                return BadRequest(new { error = "Name and Email are required." });
            }

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        [HttpPost]
        [Route("api/cars")]
        [Consumes("application/json")]
        public async Task<IActionResult> CreateFromJson([FromBody] Car car)
        {
            if (car == null || car.PersonId == null || car.PersonId == 0)
            {
                return BadRequest(new { error = "Invalid car data or missing PersonId." });
            }

            // Optional: validate that the person exists
            var personExists = await _context.People.AnyAsync(p => p.Id == car.PersonId);
            if (!personExists)
            {
                return BadRequest(new { error = "PersonId does not match any existing person." });
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        [HttpGet("persons/{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await _context.People
                .Include(p => p.Cars)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
                return NotFound();

            var dto = new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                Cars = person.Cars.Select(c => new CarDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    Make = c.Make,
                    Year = c.Year
                }).ToList()
            };

            return Ok(dto);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
                return NotFound();

            var dto = new CarDto
            {
                Id = car.Id,
                Model = car.Model,
                Make = car.Make,
                Year = car.Year,
                Color = car.Color,
                Price = (double)car.Price,
                PersonId = car.PersonId ?? 0,
                PersonName = car.Person?.Name
            };

            return Ok(dto);
        }
        [HttpPut("{id}")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateCar(int id, [FromBody] Car updatedCar)
        {
            if (id != updatedCar.Id)
            {
                return BadRequest(new { error = "Car ID in URL does not match ID in body." });
            }

            var existingCar = await _context.Cars.FindAsync(id);
            if (existingCar == null)
            {
                return NotFound(new { error = $"Car with ID {id} not found." });
            }

            // Update fields
            existingCar.Model = updatedCar.Model;
            existingCar.Make = updatedCar.Make;
            existingCar.Year = updatedCar.Year;
            existingCar.Color = updatedCar.Color;
            existingCar.Price = updatedCar.Price;
            existingCar.PersonId = updatedCar.PersonId;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // or Ok(existingCar) if you want to return the updated object
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { error = "Failed to update car.", details = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound(new { error = $"Car with ID {id} not found." });
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent(); // or Ok(new { message = "Car deleted." })
        }


    }
}
