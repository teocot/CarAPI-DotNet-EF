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
    public class PurchasesController : Controller
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _purchaseService.GetAllAsync();
            return View(purchases);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var purchase = await _purchaseService.GetByIdAsync(id.Value);
            if (purchase == null) return NotFound();

            return View(purchase);
        }

        public async Task<IActionResult> Create()
        {
            var people = await _purchaseService.GetPeopleAsync();
            ViewData["PersonId"] = new SelectList(people, "Id", "Name");

            var availableCars = _context.Cars.Where(c => c.Purchase == null).ToList();
            ViewData["CarId"] = new SelectList(cars, "Id", "Make");

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _purchaseService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }

            ViewData["PersonId"] = new SelectList(await _purchaseService.GetPeopleAsync(), "Id", "Name", model.PersonId);
            ViewData["CarId"] = new SelectList(await _purchaseService.GetAvailableCarsAsync(), "Id", "Make", model.CarId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var purchase = await _purchaseService.GetByIdAsync(id.Value);
            if (purchase == null) return NotFound();

            var viewModel = new PurchaseViewModel
            {
                purchaseId = purchase.Id,
                PurchaseDate = purchase.PurchaseDate,
                PersonId = purchase.PersonId,
                CarId = purchase.CarId
            };

            var people = await _purchaseService.GetPeopleAsync();
            ViewData["PersonId"] = new SelectList(people, "Id", "Name", viewModel.PersonId);

            var availableCars = await _purchaseService.GetAvailableCarsAsync();

            // Ensure current car is included even if it's already purchased
            if (!availableCars.Any(c => c.Id == viewModel.CarId))
            {
                var currentCar = await _purchaseService.GetCarByIdAsync(viewModel.CarId);
                if (currentCar != null)
                {
                    availableCars.Add(currentCar);
                }
            }

            ViewData["CarId"] = new SelectList(
                availableCars.Select(c => new { c.Id, Name = $"{c.Make} {c.Model}" }),
                "Id", "Name", viewModel.CarId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseViewModel model)
        {
            if (id != model.purchaseId) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _purchaseService.UpdateAsync(id, model);
                if (!success) return NotFound();

                return RedirectToAction(nameof(Index));
            }

            ViewData["PersonId"] = new SelectList(await _purchaseService.GetPeopleAsync(), "Id", "Name", model.PersonId);
            ViewData["CarId"] = new SelectList(await _purchaseService.GetAvailableCarsAsync(), "Id", "Make", model.CarId);
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var purchase = await _purchaseService.GetByIdAsync(id.Value);
            if (purchase == null) return NotFound();

            return View(purchase);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _purchaseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> PurchaseExists(int id)
        {
            return await _purchaseService.ExistsAsync(id);
        }

    }
}
