using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryLab2.Areas.Identity.Data;
using LibraryLab2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace LibraryLab2.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            expireReservations();
            return View(await _context.Book.ToListAsync());
        }

        // Displayking search page
        public async Task<IActionResult> ShowSearchForm()
        {
            expireReservations();
            return View();
        }

        // Displayking search results
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            expireReservations();
            return View("Index", await _context.Book.Where(j => j.Title.Contains(SearchPhrase)).ToListAsync());
        }

        // When user is logged he can display his/her reservations
        [Authorize]
        public async Task<IActionResult> Reservations()
        {

            expireReservations();
            return View(await _context.Book.Where(j => j.User == this.User.Identity.Name && j.Leased == null).ToListAsync());
        }

        // GET: Books/Details/5    (Not used)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

    
           
        // User can reserve book
        // - date of next day is stored because then the reservation will expire
        // - 
        public async Task<IActionResult> Reserve(int id)
        {
            expireReservations();
            if (_context.Book == null)
            {
                return Problem("LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                
                    DateTime date = DateTime.Today;
                    DateTime newDate = date.AddDays(1);
                    book.Reserved = newDate.ToShortDateString();
                    var name = User.Identity.Name;
                    book.User = name;
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Librarian can rent a book that is reserved
        public async Task<IActionResult> Rent(int id)
        {
            expireReservations();
            if (_context.Book == null)
            {
                return Problem("'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {

                book.Leased = book.User;
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Librarian can mark when a book is returned
        public async Task<IActionResult> Return(int id)
        {
            expireReservations();
            if (_context.Book == null)
            {
                return Problem("'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {

                book.Leased = null;
                book.User = null;
                book.Reserved = null;
            }


            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // User can delete reservations he did
        public async Task<IActionResult>DeleteReservation(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'LibraryContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                book.Reserved = null;
                book.User = null;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Reservations));
        }
        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Publisher,Author")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Publisher,Author,Date,User,Reserved,Leased")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return _context.Book.Any(e => e.Id == id);
        }

        // Function that updates reservations
        private void expireReservations()
        {
            string todaysDate = DateTime.Today.ToShortDateString();
            foreach (var book in _context.Book)
            {
                if (string.Compare(book.Reserved, todaysDate) < 0)
                {
                    book.Reserved = null;
                    book.User = null;
                }
            }
            return;
        }
    }
}
