using CarAPI.Models;

namespace CarAPI.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<List<Purchase>> GetAllAsync();
        Task<Purchase?> GetByIdAsync(int id);
        Task<bool> CreateAsync(PurchaseViewModel model);
        Task<bool> UpdateAsync(int id, PurchaseViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<Person>> GetPeopleAsync();
        Task<List<Car>> GetAvailableCarsAsync();
        Task<Car> GetCarByIdAsync(int carId);
    }
}
