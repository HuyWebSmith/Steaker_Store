using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Models;

namespace Steaker_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            if (_context == null)
            {
                throw new Exception("Database context is null. Check if it is initialized properly.");
            }

            var menuItemsQuery = _context.Menus.Include(m => m.Images).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                menuItemsQuery = menuItemsQuery.Where(m => m.Name.Contains(searchString));
            }

            var menuItems = await menuItemsQuery.ToListAsync();
            return View(menuItems);
        }
    }
}
