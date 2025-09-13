using CarAPI.Data;
using CarAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarAPI.ApiControllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PurchasesApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchasesApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/purchases
        [HttpGet]
        public async Task<IActionResult> GetAllPurchases()
        {
            var purchases = await _context.Purchases
                .Include(p => p.Buyer)
                .Include(p => p.Car)
                .ToListAsync();

            return Ok(purchases);
        }

        // GET: api/purchases/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Buyer)
                .Include(p => p.Car)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
                return NotFound();

            return Ok(purchase);
        }

        // POST: api/purchases
        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] Purchase purchase)
        {
            if (purchase == null || purchase.PersonId <= 0 || purchase.CarId <= 0)
            {
                return BadRequest(new { error = "PersonId and CarId are required." });
            }

            var personExists = await _context.People.AnyAsync(p => p.Id == purchase.PersonId);
            if (!personExists)
            {
                return BadRequest(new { error = $"Person with ID {purchase.PersonId} does not exist." });
            }

            var car = await _context.Cars
                .Include(c => c.Purchase)
                .FirstOrDefaultAsync(c => c.Id == purchase.CarId);

            if (car == null)
            {
                return BadRequest(new { error = $"Car with ID {purchase.CarId} does not exist." });
            }

            if (car.Purchase != null)
            {
                return BadRequest(new { error = "Car is already purchased." });
            }

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPurchase), new { id = purchase.Id }, purchase);
        }

        // PUT: api/purchases/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, [FromBody] Purchase updated)
        {
            if (id != updated.Id)
                return BadRequest(new { error = "ID mismatch." });

            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return NotFound();

            purchase.PurchaseDate = updated.PurchaseDate;
            purchase.PersonId = updated.PersonId;
            purchase.CarId = updated.CarId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/purchases/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return NotFound();

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
