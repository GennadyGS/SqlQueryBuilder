# SqlQueryBuilder

[![NuGet](https://img.shields.io/nuget/v/GennadyGS.SqlQueryBuilder.svg)](https://www.nuget.org/packages/GennadyGS.SqlQueryBuilder)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue.svg)](https://dotnet.microsoft.com/)
[![GitHub stars](https://img.shields.io/github/stars/GennadyGS/SqlQueryBuilder.svg)](https://github.com/GennadyGS/SqlQueryBuilder/stargazers)

A powerful, type-safe SQL query builder for .NET that leverages C# string interpolation to create parameterized SQL queries with automatic SQL injection protection.

## Features

- **SQL Injection Protection**: Automatically parameterizes interpolated values
- **Type-Safe**: Full IntelliSense support with compile-time checking
- **Simple API**: Natural C# string interpolation syntax
- **Composable**: Easily combine and nest query builders
- **Flexible Formatting**: Support for literal strings and custom parameter names
- **Metadata Support**: Attach additional information to queries
- **High Performance**: Minimal overhead with efficient string building
- **Zero Dependencies**: Lightweight library with no external dependencies

## Quick Start

### Installation

Install via NuGet Package Manager:

```powershell
Install-Package GennadyGS.SqlQueryBuilder
```

Or via .NET CLI:

```bash
dotnet add package GennadyGS.SqlQueryBuilder
```

### Basic Usage

```csharp
using SqlQueryBuilders;

// Simple parameterized query
SqlQueryBuilder query = $"SELECT * FROM Users WHERE Id = {userId} AND Status = {status}";

string sql = query.GetQuery();
// Result: "SELECT * FROM Users WHERE Id = @p1 AND Status = @p2"

var parameters = query.GetParameters();
// Result: { ["p1"] = userId, ["p2"] = status }
```

## Examples

### Basic Query Building

```csharp
var userId = 123;
var status = "Active";

SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE UserId = {userId} AND Status = {status}";

var (query, parameters) = queryBuilder.GetQueryAndParameters();
// query: "SELECT * FROM Orders WHERE UserId = @p1 AND Status = @p2"
// parameters: { ["p1"] = 123, ["p2"] = "Active" }
```

### Query Composition

Easily compose complex queries by combining multiple SqlQueryBuilder instances:

```csharp
SqlQueryBuilder innerQuery = $"SELECT OrderId FROM Orders WHERE UserId = {userId}";
SqlQueryBuilder outerQuery = $"SELECT * FROM OrderDetails WHERE OrderId IN ({innerQuery})";

var (sql, parameters) = outerQuery.GetQueryAndParameters();
// sql: "SELECT * FROM OrderDetails WHERE OrderId IN (SELECT OrderId FROM Orders WHERE UserId = @p1)"
// parameters: { ["p1"] = userId }
```

### Literal String Formatting

Use the `:l` format specifier to include values as literal strings (use with caution):

```csharp
var tableName = "Orders";
var userId = 123;

SqlQueryBuilder query = $"SELECT * FROM {tableName:l} WHERE UserId = {userId}";

var (sql, parameters) = query.GetQueryAndParameters();
// sql: "SELECT * FROM Orders WHERE UserId = @p1"
// parameters: { ["p1"] = 123 }
```

### Custom Parameter Prefixes

Customize parameter names for better integration with your ORM:

```csharp
SqlQueryBuilder query = $"SELECT * FROM Users WHERE Id = {userId}";

var (sql, parameters) = query.GetQueryAndParameters("param");
// sql: "SELECT * FROM Users WHERE Id = @param1"
// parameters: { ["param1"] = userId }
```

### Query Concatenation

Combine multiple query parts using standard string concatenation:

```csharp
SqlQueryBuilder baseQuery = $"SELECT * FROM Orders WHERE UserId = {userId}";
SqlQueryBuilder extendedQuery = baseQuery + $" AND CreatedDate > {startDate}";

var (sql, parameters) = extendedQuery.GetQueryAndParameters();
// sql: "SELECT * FROM Orders WHERE UserId = @p1 AND CreatedDate > @p2"
// parameters: { ["p1"] = userId, ["p2"] = startDate }
```

### Metadata Support

Attach additional metadata to your queries for logging, caching, or other purposes:

```csharp
var queryBuilder = ((SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {orderId}")
    .AddMetadata("Operation", "GetOrder")
    .AddMetadata("CacheKey", $"order_{orderId}");

var metadata = queryBuilder.Metadata;
// metadata: { ["Operation"] = "GetOrder", ["CacheKey"] = "order_123" }
```

### Working with ORMs

#### Entity Framework Core

```csharp
var minAmount = 1000;
var status = "Completed";

SqlQueryBuilder queryBuilder = $"""
    SELECT o.*, c.Name as CustomerName
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerId = c.Id
    WHERE o.Amount >= {minAmount} AND o.Status = {status}
    """;

var (query, parameters) = queryBuilder.GetQueryAndParameters();
var orders = context.Orders
    .FromSqlRaw(query, parameters.Values.ToArray())
    .ToList();
```

#### Dapper

```csharp
var queryBuilder = $"SELECT * FROM Products WHERE CategoryId = {categoryId} AND Price > {minPrice}";

var (query, parameters) = queryBuilder.GetQueryAndParameters();
var products = connection.Query<Product>(
    query,
    parameters
);
```

## Security

SqlQueryBuilder automatically protects against SQL injection by parameterizing all interpolated values. Values are never directly concatenated into the SQL string, ensuring your queries are safe by default.

```csharp
var userInput = "'; DROP TABLE Users; --";
SqlQueryBuilder query = $"SELECT * FROM Users WHERE Name = {userInput}";

// Safe! Results in: "SELECT * FROM Users WHERE Name = @p1"
// Parameter: { ["p1"] = "'; DROP TABLE Users; --" }
```

## Supported Frameworks

- .NET 6.0, .NET 8.0

## API Reference

### Core Methods

| Method | Description |
|--------|-------------|
| `GetQuery()` | Returns the parameterized SQL query string |
| `GetParameters()` | Returns the parameter dictionary |
| `GetQueryAndParameters()` | Returns both query and parameters as a tuple |
| `GetQueryAndParameters(string prefix)` | Returns query and parameters with custom parameter prefix |
| `AddMetadata(string key, object? value)` | Adds metadata to the query builder |

### Format Specifiers

| Format | Description | Example |
|--------|-------------|---------|
| Default | Parameterized value | `{userId}` → `@p1` |
| `:l` | Literal string | `{tableName:l}` → `TableName` |
| `:p` | Explicit parameter | `{value:p}` → `@p1` |

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

- Inspired by the need for type-safe SQL query building in .NET
- Built with modern C# interpolated string handlers
- Designed for developer productivity and application security
