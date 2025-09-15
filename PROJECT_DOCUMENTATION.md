# CSV Import Demo - Project Documentation

## 📋 Project Overview

This project is a **full-stack web application** that demonstrates CSV file upload, validation, and database storage functionality. It consists of an ASP.NET Core Web API backend and a React frontend, with comprehensive unit and integration testing.

## 🏗️ Project Structure

```
CsvImportDemo/
├── backend/                          # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── ProductsController.cs     # API endpoints
│   ├── Data/
│   │   └── AppDbContext.cs          # Entity Framework context
│   ├── Models/
│   │   └── Product.cs               # Product entity model
│   ├── Repositories/
│   │   ├── IProductRepository.cs    # Repository interface
│   │   └── ProductRepository.cs     # Repository implementation
│   ├── Services/
│   │   ├── IProductService.cs       # Service interface
│   │   ├── ProductService.cs        # Service implementation
│   │   └── CsvImportResult.cs       # Import result model
│   ├── Validators/
│   │   ├── IProductValidator.cs     # Validator interface
│   │   └── ProductValidator.cs      # Validation logic
│   ├── Program.cs                   # Application startup
│   ├── appsettings.json            # Configuration
│   └── CsvImportDemo.csproj        # Project file
├── frontend/                        # React Application
│   ├── src/
│   │   ├── App.js                  # Main React component
│   │   ├── App.css                 # Styling
│   │   └── index.js                # Entry point
│   ├── package.json                # Dependencies
│   └── README.md                   # Frontend docs
├── CsvImportDemo.Tests/             # XUnit Test Project
│   ├── ProductServiceTests.cs      # Unit tests with Moq
│   ├── ProductServiceIntegrationTests.cs # Integration tests
│   └── CsvImportDemo.Tests.csproj  # Test project file
├── README.md                        # Main project documentation
└── PROJECT_DOCUMENTATION.md        # This file
```

## 🛠️ Technologies Used

### Backend (ASP.NET Core)
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database operations
- **SQLite** - Lightweight, file-based database
- **Swagger/OpenAPI** - API documentation
- **CORS** - Cross-origin resource sharing

### Frontend (React)
- **React 18** - Frontend framework
- **JavaScript ES6+** - Modern JavaScript
- **CSS3** - Styling
- **Fetch API** - HTTP requests

### Testing
- **XUnit** - Testing framework
- **Moq** - Mocking framework
- **Entity Framework InMemory** - In-memory database for testing

### Development Tools
- **Node.js & npm** - Frontend package management
- **.NET CLI** - Backend development tools
- **Visual Studio Code** - IDE (recommended)

## 🔄 How It Works

### 1. **CSV Upload Process**
```
User uploads CSV → React Frontend → ASP.NET API → Validation → Database Storage
```

### 2. **Backend Flow**
1. **Controller** receives CSV file via HTTP POST
2. **Service** processes the CSV and validates each record
3. **Validator** checks business rules and data annotations
4. **Repository** saves valid records to SQLite database
5. **Response** returns import results (success/error counts)

### 3. **Frontend Flow**
1. **File Input** allows user to select CSV file
2. **Upload** sends file to backend API
3. **Validation** displays real-time validation results
4. **Success Message** confirms data saved to database
5. **Data Display** shows imported products

### 4. **Database Schema**
```sql
Products Table:
- Id (Primary Key, Auto-increment)
- Name (Required, Max 100 chars)
- Description (Max 500 chars)
- Price (Required, Decimal)
- Stock (Required, Integer)
- Category (Max 50 chars)
```

## 🧪 Testing Strategy

### Unit Tests (ProductServiceTests.cs)
- **Purpose**: Test individual methods in isolation
- **Tools**: XUnit + Moq for mocking dependencies
- **Coverage**: Service layer logic, validation, error handling
- **Tests**: 7 unit tests covering all service methods

### Integration Tests (ProductServiceIntegrationTests.cs)
- **Purpose**: Test complete workflows with real database
- **Tools**: XUnit + Entity Framework InMemory
- **Coverage**: End-to-end functionality, database persistence
- **Tests**: 5 integration tests covering real scenarios

## 🚀 How to Run the Project

### Prerequisites
- **.NET 9.0 SDK** - `brew install dotnet-sdk` (macOS)
- **Node.js 18+** - Download from nodejs.org
- **Git** - For version control

### Backend Setup
```bash
cd backend
dotnet restore
dotnet run --urls "https://localhost:7000;http://localhost:5000"
```

### Frontend Setup
```bash
cd frontend
npm install
npm start
```

### Access Points
- **API**: https://localhost:7000/swagger
- **Frontend**: http://localhost:3000
- **Database**: CsvImportDemo.db (SQLite file)

## 🧪 Running XUnit Tests

### Command to Run All Tests
```bash
cd CsvImportDemo.Tests
dotnet test --verbosity normal
```

### Command to Run Tests with Detailed Output
```bash
dotnet test --verbosity detailed
```

### Command to Run Specific Test Class
```bash
dotnet test --filter "ClassName=ProductServiceTests"
```

### Command to Run Tests and Generate Coverage Report
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Test Results Summary

**Total Tests: 12**
- ✅ **Unit Tests**: 7 tests (using Moq mocks)
- ✅ **Integration Tests**: 5 tests (using InMemory database)
- ✅ **All Tests Pass**: 100% success rate
- ✅ **Coverage**: Service layer, repository layer, validation, CSV import

## 🔧 Key Features Implemented

### ✅ CSV Upload & Validation
- File upload via React frontend
- Server-side CSV parsing
- Data validation with custom rules
- Error reporting for invalid records

### ✅ Database Integration
- Entity Framework Core with SQLite
- Repository pattern implementation
- Automatic database creation
- Data persistence verification

### ✅ API Endpoints
- `GET /api/products` - List all products
- `POST /api/products` - Create single product
- `POST /api/products/import-csv` - Import CSV file
- `GET /api/products/{id}` - Get product by ID
- `DELETE /api/products/{id}` - Delete product

### ✅ Frontend Features
- Modern React UI with file upload
- Real-time validation feedback
- Success/error message display
- Imported data visualization
- Responsive design

### ✅ Testing Coverage
- Unit tests with mocking
- Integration tests with real database
- Validation testing
- Error handling verification
- CSV import workflow testing

## 🎯 Acceptance Criteria Met

✅ **Minimal ASP.NET Core Web API** - Complete with CSV upload endpoint  
✅ **CSV Validation** - Each record validated with business rules  
✅ **Database Storage** - Valid records saved to SQLite database  
✅ **React Frontend** - Modern UI for CSV upload and data display  
✅ **Clear Separation** - Backend and frontend in separate folders  
✅ **Unit Tests** - XUnit tests with Moq for ProductService  
✅ **Repository Interactions** - All database operations tested  

## 📝 Sample CSV Format

```csv
Name,Description,Price,Stock,Category
Laptop,High-performance laptop,1299.99,25,Electronics
Smartphone,Latest model smartphone,899.99,50,Electronics
Book,Programming guide,49.99,100,Books
```

## 🔍 Troubleshooting

### Common Issues
1. **Port conflicts**: Kill existing processes with `sudo pkill -f dotnet`
2. **Database locked**: Ensure only one instance of the app is running
3. **CORS errors**: Verify backend is running on correct ports
4. **Build failures**: Run `dotnet clean` and `dotnet restore`

### Useful Commands
```bash
# Kill all dotnet processes
sudo pkill -f dotnet

# Check running processes
ps aux | grep dotnet

# Clean and rebuild
dotnet clean && dotnet build

# Check database file
ls -la *.db
```

---

**Project Status**: ✅ Complete and Fully Tested  
**Last Updated**: January 2025  
**Test Coverage**: 100% Pass Rate (12/12 tests)
