using CatalogService.Data;

namespace CatalogService.Services.Categories
{
    public interface ICategoriesService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync(CancellationToken token);

        Task<Category?> GetAsync(int id, CancellationToken token);

        Task<int> CreateAsync(Category item, CancellationToken token);

        Task<Category> UpdateAsync(Category item, Category value, CancellationToken token);

        Task<int> DeleteAsync(Category item, CancellationToken token);
    }
}
