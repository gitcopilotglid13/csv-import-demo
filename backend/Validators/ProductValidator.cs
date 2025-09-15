using System.ComponentModel.DataAnnotations;
using CsvImportDemo.Models;

namespace CsvImportDemo.Validators
{
    public class ProductValidator : IProductValidator
    {
        public async Task<ValidationResult> ValidateAsync(Product product)
        {
            var result = new ValidationResult(true, product);

            if (product == null)
            {
                return result.AddError("Product cannot be null");
            }

            // Validate using data annotations
            var validationContext = new ValidationContext(product);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            if (!Validator.TryValidateObject(product, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    result.AddError(validationResult.ErrorMessage ?? "Validation error");
                }
            }

            // Custom business logic validations
            if (!string.IsNullOrWhiteSpace(product.Name) && product.Name.Length < 2)
            {
                result.AddError("Product name must be at least 2 characters long");
            }

            if (product.Price <= 0)
            {
                result.AddError("Product price must be greater than 0");
            }

            if (product.Stock < 0)
            {
                result.AddError("Product stock cannot be negative");
            }

            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<ValidationResult>> ValidateRangeAsync(IEnumerable<Product> products)
        {
            var results = new List<ValidationResult>();

            foreach (var product in products)
            {
                var result = await ValidateAsync(product);
                results.Add(result);
            }

            return results;
        }
    }
}
