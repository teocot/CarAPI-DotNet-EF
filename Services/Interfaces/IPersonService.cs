namespace CarAPI.Services.Interfaces
{
    using CarAPI.Models;

    public interface IPersonService
    {
        Task<PersonDto?> GetPersonWithCarsAsync(int id);
        Task<List<Person>> GetAllPeopleAsync();

    }
}
