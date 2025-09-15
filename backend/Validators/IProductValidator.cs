using CsvImportDemo.Models;

namespace CsvImportDemo.Validators
{
    public interface IProductValidator
    {
        Task<ValidationResult> ValidateAsync(Product product);
        Task<IEnumerable<ValidationResult>> ValidateRangeAsync(IEnumerable<Product> products);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Product? Product { get; set; }

        public ValidationResult(bool isValid, Product? product = null)
        {
            IsValid = isValid;
            Product = product;
        }

        public ValidationResult AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
            return this;
        }
    }
}
