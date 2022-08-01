# SqlQueryBuilder
Utility class for building SQL queries with parameters based on C# string interpolation.

## Examples
```csharp
// Building SQL query with parameters
SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

Assert.Equal(
    "SELECT * FROM Orders WHERE Id = @p1", 
    queryBuilder.GetQuery());
Assert.Equal(
    new Dictionary<string, object?> { ["p1"] = 123 }, 
    queryBuilder.GetParameters());

// Composing SQL queries with parameters
SqlQueryBuilder innerQueryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";
SqlQueryBuilder outerQueryBuilder = 
    $"SELECT * FROM ({innerQueryBuilder}) src WHERE IsValid = {true}";

Assert.Equal(
    "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src WHERE IsValid = @p2",
    outerQueryBuilder.GetQuery());
Assert.Equal(
    new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true }, 
    outerQueryBuilder.GetParameters());

// Table name should be interpreted as inline literal string, rather than as parameter
const string tableName = "Orders";
SqlQueryBuilder queryBuilder = $"SELECT * FROM {tableName.AsLiteral()} WHERE Id = {123}";

var (query, parameters) = queryBuilder.GetQueryAndParameters();

Assert.Equal("SELECT * FROM Orders WHERE Id = @p1", query);
Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123 }, parameters);

// Building SQL query with specified parameter name prefix
SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

var (query, parameters) = queryBuilder.GetQueryAndParameters("param");

Assert.Equal("SELECT * FROM Orders WHERE Id = @param1", query);
Assert.Equal(new Dictionary<string, object?> { ["param1"] = 123 }, parameters);

// Building SQL query with metadata
var queryBuilder = ((SqlQueryBuilder)$"SELECT * FROM Orders WHERE Id = {123}")
    .AddMetadata("DbName", "Db1");

Assert.Equal(
    "SELECT * FROM Orders WHERE Id = @p1",
    queryBuilder.GetQuery());
Assert.Equal(
    new Dictionary<string, object?> { ["p1"] = 123 },
    queryBuilder.GetParameters());
Assert.Equal(
    new Dictionary<string, object?> { ["DbName"] = "Db1" },
    queryBuilder.Metadata);
```