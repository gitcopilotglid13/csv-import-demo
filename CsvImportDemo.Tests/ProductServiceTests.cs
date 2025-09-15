using CsvImportDemo.Models;
using CsvImportDemo.Repositories;
using CsvImportDemo.Services;
using CsvImportDemo.Validators;
using Moq;
using Xunit;

namespace CsvImportDemo.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<IProductValidator> _mockValidator;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockValidator = new Mock<IProductValidator>();
            _productService = new ProductService(_mockRepository.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProductsFromRepository()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 25, Category = "Electronics" },
                new Product { Id = 2, Name = "Smartphone", Description = "Latest model smartphone", Price = 899.99m, Stock = 50, Category = "Electronics" }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.Equal(expectedProducts, result);
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductFromRepository()
        {
            // Arrange
            var productId = 1;
            var expectedProduct = new Product { Id = productId, Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 25, Category = "Electronics" };

            _mockRepository.Setup(r => r.GetByIdAsync(productId))
                          .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            Assert.Equal(expectedProduct, result);
            _mockRepository.Verify(r => r.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_WithValidProduct_ShouldCallValidatorAndRepository()
        {
            // Arrange
            var product = new Product { Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 25, Category = "Electronics" };
            var createdProduct = new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 25, Category = "Electronics" };
            var validationResult = new ValidationResult(true, product);

            _mockValidator.Setup(v => v.ValidateAsync(product))
                         .ReturnsAsync(validationResult);
            _mockRepository.Setup(r => r.AddAsync(product))
                          .ReturnsAsync(createdProduct);

            // Act
            var result = await _productService.CreateProductAsync(product);

            // Assert
            Assert.Equal(createdProduct, result);
            _mockValidator.Verify(v => v.ValidateAsync(product), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(product), Times.Once);
        }

        [Fact]
        public async Task CreateProductAsync_WithInvalidProduct_ShouldThrowArgumentException()
        {
            // Arrange
            var product = new Product { Name = "", Description = "High-performance laptop", Price = 1299.99m, Stock = 25, Category = "Electronics" };
            var validationResult = new ValidationResult(false, product);
            validationResult.AddError("Name is required");

            _mockValidator.Setup(v => v.ValidateAsync(product))
                         .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(product));
            Assert.Contains("Validation failed", exception.Message);
            Assert.Contains("Name is required", exception.Message);
            _mockValidator.Verify(v => v.ValidateAsync(product), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldCallRepository()
        {
            // Arrange
            var productId = 1;
            _mockRepository.Setup(r => r.DeleteAsync(productId))
                          .ReturnsAsync(true);

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(productId), Times.Once);
        }

        [Fact]
        public async Task ImportProductsFromCsvAsync_WithValidCsv_ShouldImportProducts()
        {
            // Arrange
            var csvContent = "Name,Description,Price,Stock,Category\nLaptop,High-performance laptop,1299.99,25,Electronics\nSmartphone,Latest model smartphone,899.99,50,Electronics";
            var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));
            
            var products = new List<Product>
            {
                new Product { Name = "Laptop", Description = "High-performance laptop", Price = 1299.99m, Stock = 25, Category = "Electronics" },
                new Product { Name = "Smartphone", Description = "Latest model smartphone", Price = 899.99m, Stock = 50, Category = "Electronics" }
            };

            var validationResults = new List<ValidationResult>
            {
                new ValidationResult(true, products[0]),
                new ValidationResult(true, products[1])
            };

            _mockValidator.Setup(v => v.ValidateRangeAsync(It.IsAny<IEnumerable<Product>>()))
                         .ReturnsAsync(validationResults);
            _mockRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>()))
                          .ReturnsAsync(products);

            // Act
            var result = await _productService.ImportProductsFromCsvAsync(csvStream);

            // Assert
            Assert.Equal(2, result.TotalRecords);
            Assert.Equal(2, result.ValidRecords);
            Assert.Equal(0, result.InvalidRecords);
            Assert.Empty(result.Errors);
            Assert.Equal(2, result.ImportedProducts.Count);
            
            _mockValidator.Verify(v => v.ValidateRangeAsync(It.IsAny<IEnumerable<Product>>()), Times.Once);
            _mockRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<Product>>()), Times.Once);
        }

        [Fact]
        public async Task ImportProductsFromCsvAsync_WithEmptyCsv_ShouldReturnError()
        {
            // Arrange
            var csvContent = "";
            var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _productService.ImportProductsFromCsvAsync(csvStream);

            // Assert
            Assert.Equal(0, result.TotalRecords);
            Assert.Equal(0, result.ValidRecords);
            Assert.Equal(0, result.InvalidRecords);
            Assert.Single(result.Errors);
            Assert.Contains("CSV file is empty or invalid", result.Errors);
        }

        [Fact]
        public async Task UpdateProductAsync_WithValidProduct_ShouldCallValidatorAndRepository()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Original", Description = "Original Description", Price = 10.00m, Stock = 5 };
            var updatedProduct = new Product { Id = 1, Name = "Updated", Description = "Updated Description", Price = 15.00m, Stock = 10 };
            var validationResult = new ValidationResult(true, updatedProduct);

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingProduct);
            _mockValidator.Setup(v => v.ValidateAsync(updatedProduct))
                         .ReturnsAsync(validationResult);
            _mockRepository.Setup(r => r.UpdateAsync(updatedProduct))
                          .ReturnsAsync(updatedProduct);

            // Act
            var result = await _productService.UpdateProductAsync(updatedProduct);

            // Assert
            Assert.Equal(updatedProduct, result);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(updatedProduct), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(updatedProduct), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_WithNonExistingProduct_ShouldThrowArgumentException()
        {
            // Arrange
            var product = new Product { Id = 999, Name = "Non-existing", Description = "Description", Price = 10.00m, Stock = 5 };

            _mockRepository.Setup(r => r.GetByIdAsync(999))
                          .ReturnsAsync((Product?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _productService.UpdateProductAsync(product));
            Assert.Contains("Product with ID 999 not found", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(It.IsAny<Product>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task UpdateProductAsync_WithInvalidProduct_ShouldThrowArgumentException()
        {
            // Arrange
            var existingProduct = new Product { Id = 1, Name = "Original", Description = "Original Description", Price = 10.00m, Stock = 5 };
            var invalidProduct = new Product { Id = 1, Name = "", Description = "Updated Description", Price = 15.00m, Stock = 10 };
            var validationResult = new ValidationResult(false, invalidProduct);
            validationResult.AddError("Name is required");

            _mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(existingProduct);
            _mockValidator.Setup(v => v.ValidateAsync(invalidProduct))
                         .ReturnsAsync(validationResult);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _productService.UpdateProductAsync(invalidProduct));
            Assert.Contains("Validation failed", exception.Message);
            Assert.Contains("Name is required", exception.Message);
            _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            _mockValidator.Verify(v => v.ValidateAsync(invalidProduct), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task CreateProductAsync_WithNullProduct_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productService.CreateProductAsync(null!));
        }

        [Fact]
        public async Task UpdateProductAsync_WithNullProduct_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _productService.UpdateProductAsync(null!));
        }
    }
}

