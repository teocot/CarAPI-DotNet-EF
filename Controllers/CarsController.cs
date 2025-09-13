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
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Cars.Include(c => c.Person);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null) return NotFound();

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            ViewBag.PersonList = new SelectList(_context.People.ToList(), "Id", "Name");
            return View(new Car());
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

        [HttpGet("api/persons/{id}")]
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
                    Make = c.Make
                }).ToList()
            };

            return Ok(dto);
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

        [HttpPost("api/persons/{personId}/cars")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddCarToPerson(int personId, [FromBody] Car car)
        {
            // Validate person existence
            var person = await _context.People.FindAsync(personId);
            if (person == null)
            {
                return NotFound(new { error = $"Person with ID {personId} not found." });
            }

            // Assign the foreign key
            car.PersonId = personId;

            // Optional: validate car fields
            if (string.IsNullOrWhiteSpace(car.Model) || string.IsNullOrWhiteSpace(car.Make) || car.Year < 1886 || car.Price <= 0)
            {
                return BadRequest(new { error = "Invalid car data. Ensure all required fields are filled correctly." });
            }

            // Save to database
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            // Return created car
            return CreatedAtAction(nameof(GetCarById), new { id = car.Id }, car);
        }

        // get car by id
        [HttpGet("api/cars/{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
                return NotFound();

            return Ok(car);
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
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

            return CreatedAtAction(nameof(Details), new { id = car.Id }, car);
        }


        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            ViewBag.PersonList = new SelectList(_context.People.ToList(), "Id", "Name", car.PersonId);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Model,Make,Year,Color,Price,PersonId")] Car car)
        {
            if (id != car.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id)) return NotFound();
                    throw;
                }
            }

            ViewBag.PersonList = new SelectList(_context.People.ToList(), "Id", "Name", car.PersonId);
            return View(car);
        }

        [HttpPut("api/cars/{id}")]
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

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null) return NotFound();

            return View(car);
        }

        [HttpDelete("api/cars/{id}")]
        public async Task<IActionResult> DeleteCarApi(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
