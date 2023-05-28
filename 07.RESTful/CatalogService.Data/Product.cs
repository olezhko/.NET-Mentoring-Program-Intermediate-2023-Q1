using System.Text.Json.Serialization;

namespace CatalogService.Data
{
    public class Product
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public int? CategoryID { get; set; }

        public decimal? UnitPrice { get; set; }

        [JsonIgnore]
        public virtual Category? Category { get; set; }
    }
}
