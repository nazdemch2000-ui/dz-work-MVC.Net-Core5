using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Dz5.Data;
using MVC_Dz5.Models;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace MVC_Dz5.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Books.Include(b => b.Images).ToList());
        }

        public IActionResult Details(int id)
        {
            var book = _context.Books.Include(b => b.Images).FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(int bookId, string text, string? author)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return RedirectToAction("Details", new { id = bookId });
            }

            var comment = new Comment
            {
                BookId = bookId,
                Text = text,
                Author = author,
                CreatedAt = DateTime.Now
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = bookId });
        }

        public IActionResult Create()
        {
            return View(new Book());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Images).FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Book book)
        {

            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> AddImage(int bookId, string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                _context.BookImages.Add(new BookImage { BookId = bookId, Url = url });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Edit), new { id = bookId });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var img = await _context.BookImages.FindAsync(imageId);
            if (img != null)
            {
                int bookId = img.BookId;             
                _context.BookImages.Remove(img);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = bookId });
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var book = _context.Books.Include(b => b.Images).FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
