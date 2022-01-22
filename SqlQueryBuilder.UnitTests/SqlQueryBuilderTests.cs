using System.Collections.Generic;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact] 
    public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder queryBuilder = "SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", queryBuilder.GetQuery());
        Assert.Empty(queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedString()
    {
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", queryBuilder.GetQuery());
        Assert.Empty(queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedStringWithParameters()
    {
        // Building SQL query with parameters
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

        Assert.Equal(
            "SELECT * FROM Orders WHERE Id = @p1", 
            queryBuilder.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123 }, 
            queryBuilder.GetParameters());
    }

    [Fact]
    public void ShouldBeComposable()
    {
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
    }

    [Fact]
    public void ShouldReturnCorrectQueryAndParameters()
    {
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters();
        
        Assert.Equal("SELECT * FROM Orders WHERE Id = @p1", query);
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123 }, parameters);
    }

    [Fact]
    public void ShouldReturnParameterNamesWithSpecifiedSuffix()
    {
        // Building SQL query with specified parameter name prefix
        SqlQueryBuilder queryBuilder = $"SELECT * FROM Orders WHERE Id = {123}";

        var (query, parameters) = queryBuilder.GetQueryAndParameters("param");

        Assert.Equal("SELECT * FROM Orders WHERE Id = @param1", query);
        Assert.Equal(new Dictionary<string, object?> { ["param1"] = 123 }, parameters);
    }
}