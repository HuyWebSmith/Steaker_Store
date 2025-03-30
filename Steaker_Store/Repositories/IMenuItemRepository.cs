using Steaker_Store.Models;

namespace Steaker_Store.Repositories
{
    public interface IMenuItemRepository
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<MenuItem> GetByIdAsync(int id);
        Task AddAsync(MenuItem menuItem);
        Task UpdateAsync(MenuItem menuItem);
        Task DeleteAsync(int id);
        Task RemoveImagesByMenuItemIdAsync(int menuItemId);
    }
}
