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
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICarService _carService;
        private readonly IPersonService _personService;

        // constructor injection
        public CarsController(AppDbContext context, ICarService carService, IPersonService personService)
        {
            _context = context;
            _carService = carService;
            _personService = personService;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.Include(c => c.Person).ToListAsync(); // returns List<Car>
            return View(cars);
        }


        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _carService.GetCarByIdAsync(id.Value);
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
            var dto = await _personService.GetPersonWithCarsAsync(id);
            if (dto == null)
                return NotFound();

            return Ok(dto);
        }

        [HttpPost("api/persons/{personId}/cars")]
        [Consumes("application/json")]
        public async Task<IActionResult> AddCarToPerson(int personId, [FromBody] Car car)
        {
            try
            {
                var createdCar = await _carService.AddCarToPersonAsync(personId, car);
                if (createdCar == null)
                    return NotFound(new { error = $"Person with ID {personId} not found." });

                return CreatedAtAction(nameof(GetCarById), new { id = createdCar.Id }, createdCar);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // get car by id
        [HttpGet("api/cars/{id}")]
        public async Task<IActionResult> GetCarById(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            return Ok(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PersonList = new SelectList(_context.People.ToList(), "Id", "Name", car.PersonId);
                return View(car);
            }

            await _carService.CreateCarAsync(car);
            return RedirectToAction(nameof(Index));
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var car = await _carService.GetCarByIdAsync(id.Value);
            if (car == null) return NotFound();

            var people = await _personService.GetAllPeopleAsync();
            ViewBag.PersonList = new SelectList(people, "Id", "Name", car.PersonId);

            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Model,Make,Year,Color,Price,PersonId")] Car car)
        {
            if (id != car.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _carService.UpdateCarAsync(car);
                if (!success) return NotFound();

                return RedirectToAction(nameof(Index));
            }

            var people = await _personService.GetAllPeopleAsync(); // optional service refactor
            ViewBag.PersonList = new SelectList(people, "Id", "Name", car.PersonId);
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var car = await _carService.GetCarWithPersonByIdAsync(id.Value);
            if (car == null) return NotFound();

            return View(car);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _carService.DeleteCarAsync(id);
            if (!success)
            {
                TempData["ErrorMessage"] = "This car cannot be deleted because it is linked to existing purchases.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Car deleted successfully.";
            return RedirectToAction(nameof(Index));
        }


        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
