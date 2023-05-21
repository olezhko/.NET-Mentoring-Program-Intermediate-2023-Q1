using CatalogService.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Categories
{
    public class CategoriesService : ICategoriesService
    {
        private readonly CatalogServiceContext _context;

        public CategoriesService(CatalogServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken token)
        {
            return await _context.Categories.AsNoTracking().ToListAsync(cancellationToken: token);
        }

        public Task<Category?> GetAsync(int id, CancellationToken token)
        {
            return _context.Categories.SingleOrDefaultAsync(m => m.CategoryID == id, cancellationToken: token);
        }

        public async Task<int> CreateAsync(Category item, CancellationToken token)
        {
            var result = _context.Categories.Add(item);
            await _context.SaveChangesAsync(token);

            return result.Entity.CategoryID;
        }

        public async Task<Category> UpdateAsync(Category item, Category value, CancellationToken token)
        {
            item.CategoryName = value.CategoryName;
            item.Description = value.Description;

            await _context.SaveChangesAsync(token);

            return item;
        }

        public async Task<int> DeleteAsync(Category item, CancellationToken token)
        {
            _context.Categories.Remove(item);
            await _context.SaveChangesAsync(token);

            return item.CategoryID;
        }
    }
}
