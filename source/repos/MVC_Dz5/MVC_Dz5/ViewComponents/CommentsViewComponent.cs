using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_Dz5.Data;
using MVC_Dz5.Models;

namespace MVC_Dz5.ViewComponents
{
    public class CommentsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public CommentsViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var comments = await _context.Comments
                .Where(c => c.BookId == bookId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            ViewBag.BookId = bookId; 

            return View(comments);
        }
        
    }
}
