# CopilotBlazor

This solution contains:
- `Client`: Blazor WebAssembly frontend
- `Server`: ASP.NET Core Web API backend
- `Client.Tests`: xUnit tests for the frontend
- `Server.Tests`: xUnit tests for the backend

## Prerequisites
- .NET 9 SDK

## Build
```
dotnet build CopilotBlazor.sln
```

## Run Backend
```
dotnet run --project Server/Server.csproj
```

## Run Frontend
```
dotnet run --project Client/Client.csproj
```

## Run All Tests
```
dotnet test CopilotBlazor.sln
```
