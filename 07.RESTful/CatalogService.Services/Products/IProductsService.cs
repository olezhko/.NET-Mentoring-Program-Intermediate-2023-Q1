using CatalogService.Data;

namespace CatalogService.Services.Products
{
    public interface IProductsService
    {
        Task<IEnumerable<Product>> GetCategoryProductsAsync(int categoryId, int page, int pageSize, CancellationToken token);

        Task<Product?> GetAsync(int productId, CancellationToken token);

        Task<int> CreateAsync(Product item, CancellationToken token);

        Task<Product> UpdateAsync(Product oldValue, Product newValue, CancellationToken token);

        Task<int> DeleteAsync(Product item, CancellationToken token);
    }
}
