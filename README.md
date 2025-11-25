# TFinanceWeb-Backend

A REST API backend built with ASP.NET Core for the TFinanceWeb React frontend application.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Getting Started

### Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5113`
- HTTPS: `https://localhost:7233`

### Swagger/OpenAPI Documentation

When running in development mode, visit `http://localhost:5113/swagger` to view the API documentation.

## Project Structure

```
├── Controllers/           # API Controllers
│   └── WeatherForecastController.cs
├── Properties/
│   └── launchSettings.json
├── appsettings.json       # Application configuration
├── appsettings.Development.json
├── Program.cs             # Application entry point
└── TFinanceWeb.Api.csproj # Project file
```

## CORS Configuration

The API is configured to allow requests from React development servers:
- `http://localhost:3000` (Create React App default)
- `http://localhost:5173` (Vite default)

## API Endpoints

### Weather Forecast

- `GET /api/WeatherForecast` - Returns a list of weather forecasts