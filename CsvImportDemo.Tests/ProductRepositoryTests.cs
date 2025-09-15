using Microsoft.EntityFrameworkCore;
using CsvImportDemo.Data;
using CsvImportDemo.Models;
using CsvImportDemo.Repositories;
using Xunit;

namespace CsvImportDemo.Tests
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new ProductRepository(_context);
        }

        [Fact]
        public async Task GetAllAsync_WithNoProducts_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithProducts_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Description = "Description 1", Price = 10.00m, Stock = 5 },
                new Product { Name = "Product 2", Description = "Description 2", Price = 20.00m, Stock = 10 }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.Name == "Product 1");
            Assert.Contains(result, p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnProduct()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Description = "Test Description", Price = 15.00m, Stock = 8 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_WithValidProduct_ShouldAddAndReturnProduct()
        {
            // Arrange
            var product = new Product { Name = "New Product", Description = "New Description", Price = 25.00m, Stock = 15 };

            // Act
            var result = await _repository.AddAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("New Product", result.Name);

            // Verify it was saved to database
            var savedProduct = await _context.Products.FindAsync(result.Id);
            Assert.NotNull(savedProduct);
            Assert.Equal("New Product", savedProduct.Name);
        }

        [Fact]
        public async Task AddRangeAsync_WithValidProducts_ShouldAddAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Description = "Description 1", Price = 10.00m, Stock = 5 },
                new Product { Name = "Product 2", Description = "Description 2", Price = 20.00m, Stock = 10 }
            };

            // Act
            var result = await _repository.AddRangeAsync(products);

            // Assert
            Assert.Equal(2, result.Count());

            // Verify all were saved to database
            var savedProducts = await _context.Products.ToListAsync();
            Assert.Equal(2, savedProducts.Count);
            Assert.Contains(savedProducts, p => p.Name == "Product 1");
            Assert.Contains(savedProducts, p => p.Name == "Product 2");
        }

        [Fact]
        public async Task UpdateAsync_WithExistingProduct_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product { Name = "Original Name", Description = "Original Description", Price = 10.00m, Stock = 5 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Modify the product
            product.Name = "Updated Name";
            product.Price = 15.00m;

            // Act
            var result = await _repository.UpdateAsync(product);

            // Assert
            Assert.Equal("Updated Name", result.Name);
            Assert.Equal(15.00m, result.Price);

            // Verify it was updated in database
            var updatedProduct = await _context.Products.FindAsync(product.Id);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Name", updatedProduct.Name);
            Assert.Equal(15.00m, updatedProduct.Price);
        }

        [Fact]
        public async Task DeleteAsync_WithExistingId_ShouldDeleteProductAndReturnTrue()
        {
            // Arrange
            var product = new Product { Name = "To Delete", Description = "Will be deleted", Price = 10.00m, Stock = 5 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            var productId = product.Id;

            // Act
            var result = await _repository.DeleteAsync(productId);

            // Assert
            Assert.True(result);

            // Verify it was deleted from database
            var deletedProduct = await _context.Products.FindAsync(productId);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
        {
            // Arrange
            var product = new Product { Name = "Test Product", Description = "Test Description", Price = 10.00m, Stock = 5 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(product.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
