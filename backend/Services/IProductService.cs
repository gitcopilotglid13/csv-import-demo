using CsvImportDemo.Models;
using CsvImportDemo.Validators;

namespace CsvImportDemo.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        Task<CsvImportResult> ImportProductsFromCsvAsync(Stream csvStream);
    }

    public class CsvImportResult
    {
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public List<Product> ImportedProducts { get; set; } = new List<Product>();
    }
}
