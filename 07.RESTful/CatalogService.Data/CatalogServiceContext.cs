using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data
{
    public class CatalogServiceContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public CatalogServiceContext()
        {

        }

        public CatalogServiceContext(DbContextOptions<CatalogServiceContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
