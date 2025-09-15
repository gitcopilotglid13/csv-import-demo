using CsvImportDemo.Data;
using CsvImportDemo.Models;
using CsvImportDemo.Repositories;
using CsvImportDemo.Services;
using CsvImportDemo.Validators;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CsvImportDemo.Tests
{
    public class ProductServiceIntegrationTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductService _productService;

        public ProductServiceIntegrationTests()
        {
            // Create in-memory database for testing
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            // Create real instances with in-memory database
            var repository = new ProductRepository(_context);
            var validator = new ProductValidator();
            _productService = new ProductService(repository, validator);
        }

        [Fact]
        public async Task CreateProductAsync_WithValidProduct_ShouldSaveToDatabase()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Laptop",
                Description = "A test laptop for unit testing",
                Price = 999.99m,
                Stock = 10,
                Category = "Electronics"
            };

            // Act
            var result = await _productService.CreateProductAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test Laptop", result.Name);
            Assert.Equal(999.99m, result.Price);

            // Verify it was saved to database
            var savedProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal("Test Laptop", savedProduct.Name);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnProductsFromDatabase()
        {
            // Arrange - Add some test data
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Description = "Description 1", Price = 100m, Stock = 5, Category = "Category 1" },
                new Product { Name = "Product 2", Description = "Description 2", Price = 200m, Stock = 10, Category = "Category 2" }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Name == "Product 1");
            Assert.Contains(result, p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetProductByIdAsync_WithExistingId_ShouldReturnProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Description = "Test Description",
                Price = 150m,
                Stock = 8,
                Category = "Test Category"
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task ImportProductsFromCsvAsync_WithValidCsv_ShouldImportProducts()
        {
            // Arrange
            var csvContent = "Name,Description,Price,Stock,Category\n" +
                           "Imported Laptop,High-performance laptop,1299.99,25,Electronics\n" +
                           "Imported Phone,Latest smartphone,899.99,50,Electronics";
            
            var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _productService.ImportProductsFromCsvAsync(csvStream);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalRecords);
            Assert.Equal(2, result.ValidRecords);
            Assert.Equal(0, result.InvalidRecords);
            Assert.Empty(result.Errors);
            Assert.Equal(2, result.ImportedProducts.Count);

            // Verify products were saved to database
            var savedProducts = await _context.Products.ToListAsync();
            Assert.Equal(2, savedProducts.Count);
            Assert.Contains(savedProducts, p => p.Name == "Imported Laptop");
            Assert.Contains(savedProducts, p => p.Name == "Imported Phone");
        }

        [Fact]
        public async Task ImportProductsFromCsvAsync_WithInvalidData_ShouldReportErrors()
        {
            // Arrange
            var csvContent = "Name,Description,Price,Stock,Category\n" +
                           ",Invalid product,999.99,10,Electronics\n" +
                           "Valid Product,Valid description,199.99,5,Electronics";
            
            var csvStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csvContent));

            // Act
            var result = await _productService.ImportProductsFromCsvAsync(csvStream);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalRecords);
            Assert.Equal(1, result.ValidRecords);
            Assert.Equal(1, result.InvalidRecords);
            Assert.NotEmpty(result.Errors);

            // Verify only valid product was saved
            var savedProducts = await _context.Products.ToListAsync();
            Assert.Single(savedProducts);
            Assert.Equal("Valid Product", savedProducts.First().Name);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

