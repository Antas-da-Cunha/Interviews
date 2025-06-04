# Database Access API

This is a C# Web API project that connects to the SQL Server database to access data from the `Advogados` and `Horas` tables.

## Endpoints

### Advogados

- `GET /api/Advogados` - Get all lawyers
- `GET /api/Advogados/{id}` - Get a lawyer by ID
- `GET /api/Advogados/Departamento/{departamento}` - Get lawyers by department

### Horas

- `GET /api/Horas` - Get all hours
- `GET /api/Horas/{id}` - Get an hour entry by ID
- `GET /api/Horas/Advogado/{id}` - Get hour entries by lawyer ID
- `GET /api/Horas/Departamento/{departamento}` - Get hour entries by department

### Dashboard

- `GET /api/Dashboard/TopAdvogados` - Get top 5 lawyers by registered minutes
- `GET /api/Dashboard/HorasPorDepartamento` - Get total minutes by department
- `GET /api/Dashboard/AdvogadosPorDepartamento` - Get count of lawyers by department

## How to Run

1. Make sure you have the .NET SDK installed
2. Run `dotnet build` to build the project
3. Run `dotnet run` to start the API
4. Navigate to https://localhost:5001/swagger to access the Swagger UI and test the endpoints

## Connection String

The application uses a read-only connection to the Azure SQL Database with the following connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:intranet-adce-server.database.windows.net,1433;Initial Catalog=Interview;User ID=apiuser;Password=S3nh@F0rte!2025;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadOnly;MultipleActiveResultSets=False;"
  }
}
```

## Database Schema

The database contains two tables:

1. `Advogados` - Contains information about lawyers
2. `Horas` - Contains information about hours registered by lawyers 