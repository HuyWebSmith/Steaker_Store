using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Models;

namespace Steaker_Store.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MenuController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IActionResult> Index(string? searchString, string? sortOrder)
        {
            var menuItemsQuery = _context.Menus.Include(m => m.Images).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                menuItemsQuery = menuItemsQuery.Where(m => m.Name.Contains(searchString));
            }

            ViewBag.SortOrder = sortOrder;

            switch (sortOrder)
            {
                case "name_asc":
                    menuItemsQuery = menuItemsQuery.OrderBy(m => m.Name);
                    break;
                case "price_asc":
                    menuItemsQuery = menuItemsQuery.OrderBy(m => m.Price);
                    break;
                case "price_desc":
                    menuItemsQuery = menuItemsQuery.OrderByDescending(m => m.Price);
                    break;
            }

            var menuItems = await menuItemsQuery.ToListAsync();
            return View(menuItems);
        }
    }
}
