using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FribergCarRentals.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BookingsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Bookings.Include(b => b.Car).Include(b => b.Customer);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
                if (customer == null)
                    return RedirectToAction("Create", "Customers");
            }

            ViewBag.CarId = new SelectList(_context.Cars.Where(c => c.IsAvailable), "Id", "Model");

            if (isAdmin)
                ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "Email");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,CarId,CustomerId,StartDate,EndDate")] Booking booking)
        {
            var user = await _userManager.GetUserAsync(User);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == user.Id);
                if (customer == null)
                    return RedirectToAction("Create", "Customers");

                booking.CustomerId = customer.Id;
                booking.ApplicationUserId = user.Id;
            }
            else
            {
                var selectedCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == booking.CustomerId);
                if (selectedCustomer != null)
                {
                    booking.ApplicationUserId = selectedCustomer.ApplicationUserId;
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Bokning skapad!";
                return RedirectToAction("Confirmation", new { id = booking.Id });
            }

            ViewBag.CarId = new SelectList(_context.Cars, "Id", "Model", booking.CarId);
            if (isAdmin)
                ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "Email", booking.CustomerId);

            return View(booking);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
                return NotFound();

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Model", booking.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FirstName", booking.CustomerId);
            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CarId,CustomerId,StartDate,EndDate")] Booking booking)
        {
            if (id != booking.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Brand", booking.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FirstName", booking.CustomerId);
            return View(booking);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
                _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }

        [Authorize]
        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var bookings = await _context.Bookings
                .Include(b => b.Car)
                .Include(b => b.Customer)
                .Where(b => b.ApplicationUserId == user.Id)
                .ToListAsync();

            return View(bookings);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Car)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                return NotFound();

            return View(booking);
        }
    }
}
