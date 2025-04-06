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

        public async Task<IActionResult> Index()
        {
            // Kiểm tra _context trước khi truy vấn
            if (_context == null)
            {
                throw new Exception("Database context is null. Check if it is initialized properly.");
            }

            var menuItems = await _context.Menus
                .Include(m => m.Images) // Load danh sách ảnh
                .ToListAsync();


            return View(menuItems);
        }
    }
}
