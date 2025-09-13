using CarAPI.Data;
using CarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAPI.ApiControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PersonsApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            if (id != updatedPerson.Id)
                return BadRequest(new { error = "ID mismatch." });

            var person = await _context.People.FindAsync(id);
            if (person == null)
                return NotFound();

            person.Name = updatedPerson.Name;
            person.Email = updatedPerson.Email;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
                return NotFound();

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
