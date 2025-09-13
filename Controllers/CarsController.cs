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
