using CarAPI.Data;
using CarAPI.Models;
using CarAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarAPI.Services
{
    public class CarService : ICarService
    {
        private readonly AppDbContext _context;

        public CarService(AppDbContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<Car>> GetAllCarsAsync()
        //{
        //    return await _context.Cars
        //        .Include(c => c.Person)
        //        .ToListAsync();
        //}

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Car> CreateCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task<bool> UpdateCarAsync(Car car)
        {
            var existing = await _context.Cars.FindAsync(car.Id);
            if (existing == null) return false;

            existing.Model = car.Model;
            existing.Make = car.Make;
            existing.Year = car.Year;
            existing.Color = car.Color;
            existing.Price = car.Price;
            existing.PersonId = car.PersonId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCarAsync(int carId)
        {
            // Check if the car is referenced in any purchases
            bool hasPurchases = await _context.Purchases.AnyAsync(c => c.CarId == carId);
            if (hasPurchases)
            {
                // Prevent deletion
                return false;
            }

            var car = await _context.Cars.FindAsync(carId);
            if (car == null)
            {
                return false;
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<Car?> AddCarToPersonAsync(int personId, Car car)
        {
            var person = await _context.People.FindAsync(personId);
            if (person == null) return null;

            if (string.IsNullOrWhiteSpace(car.Model) ||
                string.IsNullOrWhiteSpace(car.Make) ||
                car.Year < 1886 || car.Price <= 0)
            {
                throw new ArgumentException("Invalid car data.");
            }

            car.PersonId = personId;
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return car;
        }
        public async Task<Car?> GetCarWithPersonByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> CarExistsAsync(int id)
        {
            return await _context.Cars.AnyAsync(c => c.Id == id);
        }



        public async Task<List<Car>?> GetCarsByPersonIdAsync(int personId)
        {
            var personExists = await _context.People.AnyAsync(p => p.Id == personId);
            if (!personExists) return null;

            return await _context.Cars
                .Where(c => c.PersonId == personId)
                .ToListAsync();
        }


        public async Task<CarDto?> GetCarDtoByIdAsync(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return null;

            return new CarDto
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
        }
        public async Task<List<CarDto>> GetAllCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.Person)
                .Select(c => new CarDto
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
        }

        public async Task<Car?> GetCarEntityByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Purchase)
                .ThenInclude(p => p.Buyer)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
