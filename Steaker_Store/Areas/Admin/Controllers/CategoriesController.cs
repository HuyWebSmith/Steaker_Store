using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Steaker_Store.Areas.Admin.Models;
using Steaker_Store.Models;
using Steaker_Store.Repositories;

namespace Steaker_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoriesController : Controller
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(IMenuItemRepository menuItemRepository, ICategoryRepository
   categoryRepository)
        {
            _menuItemRepository = menuItemRepository;
            _categoryRepository = categoryRepository;
        }
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<IActionResult> Index()
        {
            var category = await _categoryRepository.GetAllAsync();
            return View(category);
        }
        public async Task<IActionResult> Display(int id)
        {

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState is NOT valid!");

                // In tất cả lỗi ra Console
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"🚨 Field: {kvp.Key} - Error: {error.ErrorMessage}");
                    }
                }

                return View(category);
            }

            Console.WriteLine($"✅ Adding category: {category.Name}");
            await _categoryRepository.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            { // Kiểm tra log
                await _categoryRepository.DeleteAsync(id);  
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
