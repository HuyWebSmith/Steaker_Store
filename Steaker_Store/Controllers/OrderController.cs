using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Models;
using System.Security.Claims;

namespace Steaker_Store.Controllers
{
    
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của user đang đăng nhập
            if (userId == null) return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Where(o => o.UserId == userId) // Chỉ lấy đơn của user hiện tại
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.MenuItem)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ApplicationUser) // Load thông tin khách hàng
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem) // Load thông tin món ăn
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của user đang đăng nhập
            if (userId == null) return RedirectToAction("Login", "Account");

            // Lấy đơn hàng cần xóa
            var order = await _context.Orders
                .Where(o => o.UserId == userId && o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return View(order); // Hiển thị view xác nhận xóa
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID của user đang đăng nhập
            if (userId == null) return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .Where(o => o.UserId == userId && o.Id == id)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            // Xóa đơn hàng
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Quay lại trang danh sách đơn hàng
        }

    }
}
