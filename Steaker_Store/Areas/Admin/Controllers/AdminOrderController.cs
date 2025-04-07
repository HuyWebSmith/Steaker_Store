using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Models;
using System.Security.Claims;

namespace Steaker_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                              .Include(o => o.ApplicationUser) // Assuming you want to include user info
                              .ToListAsync();

            return View(orders);  // Pass orders to the view
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.ApplicationUser)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order); // Trả về thông tin đơn hàng cho view Details
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);  // Hiển thị view xác nhận xóa
        }

        // POST: AdminOrder/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order); // Xóa đơn hàng khỏi cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Quay lại trang danh sách đơn hàng
        }

    }
}
