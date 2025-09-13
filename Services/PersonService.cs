namespace CarAPI.Services
{
    using CarAPI.Data;
    using CarAPI.Models;
    using CarAPI.Services.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class PersonService : IPersonService
    {
        private readonly AppDbContext _context;

        public PersonService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PersonDto?> GetPersonWithCarsAsync(int id)
        {
            var person = await _context.People
                .Include(p => p.Cars)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null) return null;

            return new PersonDto
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
        }
        public async Task<List<Person>> GetAllPeopleAsync()
        {
            return await _context.People.ToListAsync();
        }
        public async Task<bool> DeletePersonAsync(int personId)
        {
            // Check if the person is referenced in any purchases
            bool hasPurchases = await _context.Purchases.AnyAsync(p => p.PersonId == personId);
            if (hasPurchases)
            {
                // Prevent deletion
                return false;
            }

            var person = await _context.People.FindAsync(personId);
            if (person == null)
            {
                return false;
            }

            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
