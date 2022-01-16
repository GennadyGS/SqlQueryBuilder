# SqlQueryBuilder
Utility class for building SQL queries with parameters based on C# string interpolation.

## Examples
```csharp
// Building SQL query with parameters
SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";
queryBuilder.GetQuery(); // "SELECT * FROM Orders WHERE Id = @p1"
queryBuilder.GetParameters(); // new Dictionary { ["p1"] = 123 }

// Composing SQL queries with parameters
SqlQueryBuilder innerQueryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";
SqlQueryBuilder queryBuilder = $"SELECT * FROM ({innerQuery}) src WHERE IsValid = {true}";
queryBuilder.GetQuery(); // "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src WHERE IsValid = @p2"
queryBuilder.GetParameters(); // new Dictionary<string, object> { ["p1"] = 123, ["p2"] = true }
```
