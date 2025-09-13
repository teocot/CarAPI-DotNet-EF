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
    public class PurchasesController : Controller
    {
        private readonly AppDbContext _context;

        public PurchasesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Purchases.Include(p => p.Buyer).Include(p => p.Car);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Buyer)
                .Include(p => p.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Name");
            ViewData["CarId"] = new SelectList(
    _context.Cars.Where(c => c.Purchase == null), "Id", "Make");

            return View();
        }

        // POST: Purchases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var purchase = new Purchase
                {
                    PurchaseDate = model.PurchaseDate,
                    PersonId = model.PersonId,
                    CarId = model.CarId
                };

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Name", model.PersonId);
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Make", model.CarId);
            return View(model);
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Buyer)
                .Include(p => p.Car)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            var viewModel = new PurchaseViewModel
            {
                purchaseId = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                PersonId = purchase.PersonId,
                CarId = purchase.CarId
            };

            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Name", viewModel.PersonId);
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Make", viewModel.CarId);

            return View(viewModel);
        }

        // POST: Purchases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseViewModel model)
        {
            if (id != model.purchaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var purchase = await _context.Purchases.FindAsync(id);
                if (purchase == null)
                {
                    return NotFound();
                }

                purchase.PurchaseDate = model.PurchaseDate;
                purchase.PersonId = model.PersonId;
                purchase.CarId = model.CarId;

                try
                {
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseExists(purchase.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["PersonId"] = new SelectList(_context.People, "Id", "Name", model.PersonId);
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Make", model.CarId);
            return View(model);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.Buyer)
                .Include(p => p.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase != null)
            {
                _context.Purchases.Remove(purchase);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseExists(int id)
        {
            return _context.Purchases.Any(e => e.Id == id);
        }
    }
}
