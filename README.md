   # CSV Import Demo

A minimal ASP.NET Core Web API with React frontend that accepts CSV uploads, validates each record, and saves valid records to a database.

## Features

- **CSV Upload**: Accept CSV files via HTTP POST
- **Data Validation**: Validate each record using data annotations and custom business rules
- **Database Storage**: Save valid records to SQL Server database using Entity Framework Core
- **Error Handling**: Comprehensive error handling and validation feedback
- **RESTful API**: Full CRUD operations for products
- **Swagger Documentation**: API documentation available at `/swagger`
- **React Frontend**: Modern, responsive web interface for CSV upload and data visualization

## Project Structure

```
CsvImportDemo/
├── backend/                  # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── ProductsController.cs
│   ├── Services/
│   │   ├── ProductService.cs
│   │   └── IProductService.cs
│   ├── Validators/
│   │   ├── ProductValidator.cs
│   │   └── IProductValidator.cs
│   ├── Repositories/
│   │   ├── ProductRepository.cs
│   │   └── IProductRepository.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Models/
│   │   └── Product.cs
│   ├── Program.cs
│   ├── CsvImportDemo.csproj
│   ├── appsettings.json
│   └── CsvImportDemo.db      # SQLite database
├── frontend/                 # React frontend application
│   ├── src/
│   │   ├── App.js
│   │   ├── App.css
│   │   └── index.js
│   ├── package.json
│   └── README.md
└── sample-products.csv
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB is used by default)
- Node.js (version 14 or higher) - for the React frontend

### Running the Application

#### Backend (ASP.NET Core API)

1. Navigate to the backend directory:
   ```bash
   cd CsvImportDemo/backend
   ```

2. Restore packages:
   ```bash
   dotnet restore
   ```

3. Run the backend API:
   ```bash
   dotnet run
   ```

4. The API will be available at:
   - API: `https://localhost:7000` (or the port shown in the console)
   - Swagger UI: `https://localhost:7000/swagger`

#### Frontend (React Application)

1. Navigate to the frontend directory:
   ```bash
   cd CsvImportDemo/frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the React development server:
   ```bash
   npm start
   ```

4. Open your browser and navigate to:
   - Frontend: `http://localhost:3000`

### Testing CSV Import

#### Using the Web Interface (Recommended)

1. Start both the backend API and React frontend (see instructions above)
2. Open the frontend at `http://localhost:3000`
3. Click "Choose CSV File" and select the sample CSV file (`sample-products.csv` from the root directory)
4. Click "Upload & Import" to process the file
5. View the import results, validation errors, and imported products in the web interface

#### Using the API Directly

1. Use the sample CSV file provided (`sample-products.csv`)
2. Send a POST request to `/api/products/import-csv` with the CSV file as form data
3. The API will return a detailed import result showing:
   - Total records processed
   - Number of valid records
   - Number of invalid records
   - List of errors (if any)
   - List of imported products

### API Endpoints

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update an existing product
- `DELETE /api/products/{id}` - Delete a product
- `POST /api/products/import-csv` - Import products from CSV file

### CSV Format

The CSV file should have the following columns:
- `Name` (required) - Product name
- `Description` (required) - Product description
- `Price` (required) - Product price (decimal)
- `Stock` (required) - Stock quantity (integer)
- `Category` (optional) - Product category

### Running Tests

```bash
dotnet test
```

## Architecture

The application follows a layered architecture:

- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic and orchestration
- **Repositories**: Data access layer
- **Validators**: Data validation logic
- **Models**: Data entities and DTOs
- **Data**: Entity Framework DbContext

This separation of concerns makes the code maintainable, testable, and follows SOLID principles.
