using Microsoft.EntityFrameworkCore;
using Steaker_Store.Models;

namespace Steaker_Store.Repositories
{
    public class EFMeunuItemRepository : IMenuItemRepository
    {
        private readonly ApplicationDbContext _context;
        public EFMeunuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync(); 
            return await _context.Menus
        .Include(p => p.Category) // Include thông tin về category 
        .ToListAsync();

        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            var menuItem = await _context.Menus.Include(p => p.Category)
                                       .FirstOrDefaultAsync(p => p.Id == id);
            if (menuItem == null)
            {
                throw new Exception($"Không tìm thấy MenuItem với ID = {id}");
            }

            return menuItem;
        }

        public async Task AddAsync(MenuItem menuItem)
        {
            _context.Menus.Add(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            _context.Menus.Update(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menuItem = await _context.Menus.FindAsync(id);
            if (menuItem != null)
            {
                _context.Menus.Remove(menuItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveImagesByMenuItemIdAsync(int menuItemId)
        {
            var images = _context.ProductImages
           .Where(img => img.MenuItemId == menuItemId) 
           .ToList();

            if (images.Any())
            {
                _context.ProductImages.RemoveRange(images);
                await _context.SaveChangesAsync();
            }
        }

    }
}
