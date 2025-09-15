using CsvImportDemo.Models;
using CsvImportDemo.Validators;
using Xunit;

namespace CsvImportDemo.Tests
{
    public class ProductValidatorTests
    {
        private readonly ProductValidator _validator;

        public ProductValidatorTests()
        {
            _validator = new ProductValidator();
        }

        [Fact]
        public async Task ValidateAsync_WithValidProduct_ShouldReturnValidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "A valid test product",
                Price = 99.99m,
                Stock = 10,
                Category = "Test Category"
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
            Assert.Equal(product, result.Product);
        }

        [Fact]
        public async Task ValidateAsync_WithNullProduct_ShouldReturnInvalidResult()
        {
            // Arrange
            Product? product = null;

            // Act
            var result = await _validator.ValidateAsync(product!);

            // Assert
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains("Product cannot be null", result.Errors);
        }

        [Fact]
        public async Task ValidateAsync_WithEmptyName_ShouldReturnInvalidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "",
                Description = "A test product",
                Price = 99.99m,
                Stock = 10
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Name"));
        }

        [Fact]
        public async Task ValidateAsync_WithShortName_ShouldReturnInvalidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "A", // Less than 2 characters
                Description = "A test product",
                Price = 99.99m,
                Stock = 10
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("at least 2 characters"));
        }

        [Fact]
        public async Task ValidateAsync_WithEmptyDescription_ShouldReturnInvalidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "",
                Price = 99.99m,
                Stock = 10
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Description"));
        }

        [Fact]
        public async Task ValidateAsync_WithZeroPrice_ShouldReturnInvalidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "A test product",
                Price = 0,
                Stock = 10
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Price") || e.Contains("greater than 0"));
        }

        [Fact]
        public async Task ValidateAsync_WithNegativePrice_ShouldReturnInvalidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "A test product",
                Price = -10.50m,
                Stock = 10
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Price") || e.Contains("greater than 0"));
        }

        [Fact]
        public async Task ValidateAsync_WithNegativeStock_ShouldReturnInvalidResult()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "A test product",
                Price = 99.99m,
                Stock = -5
            };

            // Act
            var result = await _validator.ValidateAsync(product);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Stock") || e.Contains("negative"));
        }


        [Fact]
        public async Task ValidateRangeAsync_WithValidProducts_ShouldReturnAllValidResults()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Description = "Description 1", Price = 10.00m, Stock = 5 },
                new Product { Name = "Product 2", Description = "Description 2", Price = 20.00m, Stock = 10 }
            };

            // Act
            var results = await _validator.ValidateRangeAsync(products);

            // Assert
            Assert.Equal(2, results.Count());
            Assert.All(results, r => Assert.True(r.IsValid));
            Assert.All(results, r => Assert.Empty(r.Errors));
        }


        [Fact]
        public async Task ValidateRangeAsync_WithEmptyCollection_ShouldReturnEmptyResults()
        {
            // Arrange
            var products = new List<Product>();

            // Act
            var results = await _validator.ValidateRangeAsync(products);

            // Assert
            Assert.Empty(results);
        }
    }
}
