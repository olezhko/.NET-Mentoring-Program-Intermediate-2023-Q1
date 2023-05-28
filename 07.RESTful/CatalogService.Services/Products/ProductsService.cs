using CatalogService.Data;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Services.Products
{
    public class ProductsService : IProductsService
    {
        private readonly CatalogServiceContext _context;

        public ProductsService(CatalogServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetCategoryProductsAsync(int categoryId, int page, int pageSize, CancellationToken token)
        {
            var items = _context.Products
                .Include(item => item.Category)
                .Where(item => item.CategoryID == categoryId)
                .Skip(page * pageSize)
                .Take(pageSize);

            return await items.ToListAsync(cancellationToken: token);
        }

        public Task<Product?> GetAsync(int productId, CancellationToken token)
        {
            return _context.Products.SingleOrDefaultAsync(m => m.ProductID == productId, cancellationToken: token);
        }

        public async Task<int> CreateAsync(Product product, CancellationToken token)
        {
            var item = _context.Products.Add(product);
            await _context.SaveChangesAsync(token);

            return item.Entity.ProductID;
        }

        public async Task<Product> UpdateAsync(Product item, Product value, CancellationToken token)
        {
            item.UnitPrice = value.UnitPrice;
            item.ProductName = value.ProductName;

            item.Category = value.Category;

            await _context.SaveChangesAsync(token);

            return item;
        }

        public async Task<int> DeleteAsync(Product item, CancellationToken token)
        {
            _context.Products.Remove(item);
            await _context.SaveChangesAsync(token);

            return item.ProductID;
        }
    }
}