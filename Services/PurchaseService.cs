using CarAPI.Data;
using CarAPI.Models;
using CarAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarAPI.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _context;

        public PurchaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Purchase>> GetAllAsync()
        {
            return await _context.Purchases.Include(p => p.Buyer).Include(p => p.Car).ToListAsync();
        }

        public async Task<Purchase?> GetByIdAsync(int id)
        {
            return await _context.Purchases.Include(p => p.Buyer).Include(p => p.Car).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> CreateAsync(PurchaseViewModel model)
        {
            var purchase = new Purchase
            {
                PurchaseDate = model.PurchaseDate,
                PersonId = model.PersonId,
                CarId = model.CarId
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(int id, PurchaseViewModel model)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null) return false;

            purchase.PurchaseDate = model.PurchaseDate;
            purchase.PersonId = model.PersonId;
            purchase.CarId = model.CarId;

            _context.Update(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null) return false;

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Purchases.AnyAsync(p => p.Id == id);
        }

        public async Task<List<Person>> GetPeopleAsync()
        {
            return await _context.People.ToListAsync();
        }

        public async Task<List<Car>> GetAvailableCarsAsync()
        {
            return await _context.Cars.Where(c => c.Purchase == null).ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int carId)
        {
            return await _context.Cars.FindAsync(carId);
        }

    }
}
