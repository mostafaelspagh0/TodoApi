# Todo List API

A clean architecture-based RESTful API for managing todo items, built with .NET 8.

## Features

- Create, retrieve, and update todo items
- Filter todos by completion status
- Clean Architecture implementation
- Comprehensive test coverage
- Swagger documentation
- Structured logging with Serilog
- In-memory database for easy setup

## Tech Stack

- .NET 8
- Entity Framework Core
- Swagger/OpenAPI
- Serilog
- xUnit & Moq for testing

## Project Structure

```
TodoApi/
├── src/
│   ├── TodoApi.Domain/        # Enterprise business rules
│   ├── TodoApi.Application/   # Application business rules
│   ├── TodoApi.Infrastructure/# External concerns
│   └── TodoApi.API/          # Web API controllers
└── tests/
    ├── TodoApi.UnitTests/
    ├── TodoApi.E2ETests/
    └── TodoApi.IntegrationTests/

```

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or VS Code

## Getting Started

1. Clone the repository:
```bash
git clone https://github.com/yourusername/TodoApi.git
cd TodoApi
```

2. Build the solution:
```bash
dotnet build
```

3. Run the tests:
```bash
dotnet test
```

4. Run the API:
```bash
cd src/TodoApi.API
dotnet run
```

The API will be available at `https://localhost:5001` (or `http://localhost:5000`).

## API Endpoints

### Create Todo
```http
POST /api/todo
Content-Type: application/json

{
    "title": "Complete project",
    "description": "Finish the todo API implementation"
}
```

### Get All Todos
```http
GET /api/todo
```

### Get Pending Todos
```http
GET /api/todo/pending
```

### Mark Todo as Completed
```http
PUT /api/todo/{id}/complete
```

## Response Examples

### Todo Item
```json
{
    "id": 1,
    "title": "Complete project",
    "description": "Finish the todo API implementation",
    "isCompleted": false,
    "createdDate": "2024-12-23T10:00:00Z"
}
```

## Development

### Database

The project uses Entity Framework Core with an in-memory database for simplicity. To switch to a different database provider:

1. Install the appropriate NuGet package
2. Update the connection string in `appsettings.json`
3. Modify the DbContext configuration in `Program.cs`

### Testing

The project includes both unit tests and integration tests:

```bash
# Run unit tests
dotnet test tests/TodoApi.UnitTests

# Run integration tests
dotnet test tests/TodoApi.IntegrationTests
```

### Logging

Logging is implemented using Serilog and writes to the console by default. Configure additional sinks in `Program.cs`.

## Clean Architecture

This project follows Clean Architecture principles:

- **Domain Layer**: Contains enterprise business rules and entities
- **Application Layer**: Contains application business rules
- **Infrastructure Layer**: Contains frameworks and external concerns
- **API Layer**: Contains controllers and configuration

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
