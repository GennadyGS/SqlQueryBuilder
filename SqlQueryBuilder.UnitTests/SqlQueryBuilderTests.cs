using System.Collections.Generic;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact] 
    public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal(expected: "SELECT * FROM Orders WHERE Id = 123", actual: query.GetQuery());
        Assert.Empty(query.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedString()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal(expected: "SELECT * FROM Orders WHERE Id = 123", actual: query.GetQuery());
        Assert.Empty(query.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedStringWithParameters()
    {
        // Building SQL query with parameters
        SqlQueryBuilder query = $"SELECT * FROM Orders WHERE Id = {123}";

        Assert.Equal(
            expected: "SELECT * FROM Orders WHERE Id = @p1", 
            actual: query.GetQuery());
        Assert.Equal(
            expected: new Dictionary<string, object?> { ["p1"] = 123 }, 
            actual: query.GetParameters());
    }

    [Fact]
    public void ShouldBeComposable()
    {
        // Composing SQL queries with parameters
        SqlQueryBuilder innerQuery = $"SELECT * FROM Orders WHERE Id = {123}";
        SqlQueryBuilder outerQuery = $"SELECT * FROM ({innerQuery}) src WHERE IsValid = {true}";

        Assert.Equal(
            expected: "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src WHERE IsValid = @p2",
            actual: outerQuery.GetQuery());
        Assert.Equal(
            expected: new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true }, 
            actual: outerQuery.GetParameters());
    }
}