using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Areas.Admin.Models;
using Steaker_Store.Enums;
using Steaker_Store.Extensions;
using Steaker_Store.Models;
using Steaker_Store.Repositories;

namespace Steaker_Store.Controllers
{
    
    public class ShoppingCartController : Controller
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult Checkout()
        {
            return View(new CheckoutViewModel());
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Customer)]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (!ModelState.IsValid || cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Cart is empty or information is missing.";
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.UtcNow,
                OrderCode = DateTime.Now.Ticks.ToString(),
                ShippingAddress = model.ShippingAddress,
                Notes = model.Notes,
                TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity),
                Status = PaymentStatusEnum.ChuaThanhToan,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    MenuItemId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");

            return View("OrderCompleted", order.Id);
        }
        [Authorize(Roles = SD.Role_Customer)]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await GetProductFromDatabase(productId);

            if (product == null)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại!";
                return Redirect(Request.Headers["Referer"].ToString()); // Quay lại trang trước đó
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            cart.AddItem(new CartItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            });

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return Redirect(Request.Headers["Referer"].ToString()); // Quay lại trang trước đó
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }
        // Các actions khác... 

        private async Task<MenuItem> GetProductFromDatabase(int productId)
        {
            // Truy vấn cơ sở dữ liệu để lấy thông tin sản phẩm 
            var product = await _menuItemRepository.GetByIdAsync(productId);
            return product;
        }
        [Authorize(Roles = SD.Role_Customer)]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart is not null)
            {
                cart.RemoveItem(productId);
                // Lưu lại giỏ hàng vào Session sau khi đã xóa mục 
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveAllFromCart()
        {
            HttpContext.Session.Remove("Cart"); // Xóa giỏ hàng khỏi session
            return RedirectToAction("Index"); // Quay lại trang giỏ hàng
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Detail(int id)
        {
            var product = _context.Menus
                .Include(p => p.Images) // Nếu sản phẩm có nhiều ảnh
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View("~/Views/ShoppingCart/Detail.cshtml",product);
        }

    }
}
