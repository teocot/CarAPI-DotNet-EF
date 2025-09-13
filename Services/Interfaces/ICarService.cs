using CarAPI.Models;

    namespace CarAPI.Services.Interfaces
{

    public interface ICarService
    {
        Task<List<CarDto>> GetAllCarsAsync();
        Task<List<Car>>? GetCarsByPersonIdAsync(int personId);
        Task<Car> CreateCarAsync(Car car);
        Task<CarDto?> GetCarDtoByIdAsync(int id);
        Task<bool> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
        Task<bool> CarExistsAsync(int id);
        Task<Car?> GetCarEntityByIdAsync(int id);
        Task<Car> GetCarByIdAsync(int id);
        Task<string?> GetCarWithPersonByIdAsync(int value);
        Task<Car> AddCarToPersonAsync(int personId, Car car);
    }

}
