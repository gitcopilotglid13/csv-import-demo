using System.Globalization;
using CsvImportDemo.Models;
using CsvImportDemo.Repositories;
using CsvImportDemo.Validators;

namespace CsvImportDemo.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductValidator _productValidator;

        public ProductService(IProductRepository productRepository, IProductValidator productValidator)
        {
            _productRepository = productRepository;
            _productValidator = productValidator;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }

            var validationResult = await _productValidator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            return await _productRepository.AddAsync(product);
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }

            var existingProduct = await _productRepository.GetByIdAsync(product.Id);
            if (existingProduct == null)
            {
                throw new ArgumentException($"Product with ID {product.Id} not found");
            }

            var validationResult = await _productValidator.ValidateAsync(product);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Validation failed: {string.Join(", ", validationResult.Errors)}");
            }

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }

        public async Task<CsvImportResult> ImportProductsFromCsvAsync(Stream csvStream)
        {
            var result = new CsvImportResult();
            var products = new List<Product>();
            var validProducts = new List<Product>();

            try
            {
                using var reader = new StreamReader(csvStream);
                var headerLine = await reader.ReadLineAsync();

                if (string.IsNullOrEmpty(headerLine))
                {
                    result.Errors.Add("CSV file is empty or invalid");
                    return result;
                }

                // Parse header to get column indices
                var headers = ParseCsvLine(headerLine);
                var nameIndex = GetColumnIndex(headers, "Name");
                var descriptionIndex = GetColumnIndex(headers, "Description");
                var priceIndex = GetColumnIndex(headers, "Price");
                var stockIndex = GetColumnIndex(headers, "Stock");
                var categoryIndex = GetColumnIndex(headers, "Category");

                if (nameIndex == -1 || descriptionIndex == -1 || priceIndex == -1 || stockIndex == -1)
                {
                    result.Errors.Add("Required columns (Name, Description, Price, Stock) not found in CSV");
                    return result;
                }

                string? line;
                int lineNumber = 1;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    result.TotalRecords++;

                    try
                    {
                        var values = ParseCsvLine(line);

                        if (values.Length < Math.Max(nameIndex, Math.Max(descriptionIndex, Math.Max(priceIndex, stockIndex))) + 1)
                        {
                            result.Errors.Add($"Line {lineNumber}: Insufficient columns");
                            result.InvalidRecords++;
                            continue;
                        }

                        var product = new Product
                        {
                            Name = values[nameIndex]?.Trim() ?? "",
                            Description = values[descriptionIndex]?.Trim() ?? "",
                            Price = decimal.Parse(values[priceIndex]?.Trim() ?? "0", CultureInfo.InvariantCulture),
                            Stock = int.Parse(values[stockIndex]?.Trim() ?? "0"),
                            Category = categoryIndex >= 0 && categoryIndex < values.Length ? values[categoryIndex]?.Trim() : null
                        };

                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Line {lineNumber}: {ex.Message}");
                        result.InvalidRecords++;
                    }
                }

                // Validate all products
                var validationResults = await _productValidator.ValidateRangeAsync(products);

                foreach (var validationResult in validationResults)
                {
                    if (validationResult.IsValid)
                    {
                        validProducts.Add(validationResult.Product!);
                    }
                    else
                    {
                        result.InvalidRecords++;
                        result.Errors.AddRange(validationResult.Errors);
                    }
                }

                // Save valid products
                if (validProducts.Any())
                {
                    await _productRepository.AddRangeAsync(validProducts);
                    result.ImportedProducts = validProducts;
                }

                result.ValidRecords = validProducts.Count;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error processing CSV: {ex.Message}");
            }

            return result;
        }

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += c;
                }
            }

            result.Add(current);
            return result.ToArray();
        }

        private static int GetColumnIndex(string[] headers, string columnName)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (string.Equals(headers[i].Trim(), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
