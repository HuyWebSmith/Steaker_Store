using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Steaker_Store.Areas.Admin.Models;
using Steaker_Store.Models;
using Steaker_Store.Repositories;

namespace Steaker_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class MenuItemController : Controller
    {
        private readonly IMenuItemRepository _menuItemRepository;
            private readonly ICategoryRepository _categoryRepository;

            public MenuItemController(IMenuItemRepository menuItemRepository, ICategoryRepository categoryRepository)
            {
                _menuItemRepository = menuItemRepository;
                _categoryRepository = categoryRepository;
            }
            
            public async Task<IActionResult> Index()
            {
            var menuItem = await _menuItemRepository.GetAllAsync();
            return View(menuItem);
            }
            public async Task<IActionResult> Add()
            {
                var categories = await _categoryRepository.GetAllAsync();

                foreach (var category in categories)
                {
                    Console.WriteLine($"ID: {category.Id} - Name: {category.Name}");
                }
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
           

                 return View();  
            }
            [HttpPost]
            public async Task<IActionResult> Add(MenuItem menuItem, List<IFormFile> imageUrls)
            {
                if (ModelState.IsValid)
                {   
                    if (imageUrls != null && imageUrls.Count > 0)
                    {
                        var imageUrlsList = await SaveImages(imageUrls);
                        menuItem.Images = imageUrlsList.Select(url => new MenuItemImage { Url = url }).ToList();
                    }

                    menuItem.Price = Convert.ToDecimal(menuItem.Price);
                    await _menuItemRepository.AddAsync(menuItem);
                    return RedirectToAction(nameof(Index));
                }

                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
                return View(menuItem);
            }


        private async Task<List<string>> SaveImages(List<IFormFile> images)
        {
            var imageUrls = new List<string>();
            var uploadsFolder = Path.Combine("wwwroot", "images");

            // Tạo thư mục nếu chưa tồn tại
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            foreach (var image in images)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var savePath = Path.Combine("wwwroot/images", fileName);

                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                imageUrls.Add("/images/" + fileName);
            }

            return imageUrls;
        }

        public async Task<IActionResult> Display(int id)
            {
                var product = await _menuItemRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return View(product);
            }

        // Hiển thị form cập nhật sản phẩm 

            public async Task<IActionResult> Update(int id)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(id);
                if (menuItem == null)
                {
                    return NotFound();
                }

                var categories = await _categoryRepository.GetAllAsync();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", menuItem.CategoryId);

                return View(menuItem);

            }
        // Update the Update method to handle a single IFormFile
        [HttpPost]
        public async Task<IActionResult> Update(int id, MenuItem menuItem, List<IFormFile> imageUrl)
        {
            // Loại bỏ xác thực ModelState cho ImageUrl
            ModelState.Remove("ImageUrl");

            if (id != menuItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingMenuItem = await _menuItemRepository.GetByIdAsync(id);
                if (existingMenuItem == null)
                {
                    return NotFound();
                }
                Console.WriteLine($"imageUrl is {(imageUrl == null ? "null" : "not null")}");
                Console.WriteLine($"imageUrl.Count = {imageUrl?.Count}");
                if (imageUrl != null && imageUrl.Any())
                {

                    existingMenuItem.Images?.Clear();
                    await _menuItemRepository.RemoveImagesByMenuItemIdAsync(existingMenuItem.Id);
                    // Lưu ảnh mới
                    var imageUrlsList = await SaveImages(imageUrl);
                    existingMenuItem.Images = imageUrlsList.Select(url => new MenuItemImage { Url = url }).ToList();
                }

                // Cập nhật thông tin khác
                existingMenuItem.Name = menuItem.Name;
                existingMenuItem.Price = menuItem.Price;
                existingMenuItem.Description = menuItem.Description;
                existingMenuItem.CategoryId = menuItem.CategoryId;

                await _menuItemRepository.UpdateAsync(existingMenuItem);
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", menuItem.CategoryId);
            return View(menuItem);
        }

             // Hiển thị form xác nhận xóa sản phẩm 
            public async Task<IActionResult> Delete(int id)
            {
                var menuItem = await _menuItemRepository.GetByIdAsync(id);
                if (menuItem == null)
                {
                    return NotFound();
                }
                return View(menuItem);
            }

            // Xử lý xóa sản phẩm
            [HttpPost, ActionName("Delete")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                await _menuItemRepository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
        
    }
}
