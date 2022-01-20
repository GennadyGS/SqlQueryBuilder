using System.Collections.Generic;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact] 
    public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", query.GetQuery());
        Assert.Empty(query.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedString()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders WHERE Id = 123";

        Assert.Equal("SELECT * FROM Orders WHERE Id = 123", query.GetQuery());
        Assert.Empty(query.GetParameters());
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedStringWithParameters()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders WHERE Id = {123}";

        Assert.Equal("SELECT * FROM Orders WHERE Id = @p1", query.GetQuery());
        Assert.Equal(new Dictionary<string, object?> { ["p1"] = 123 }, query.GetParameters());
    }

    [Fact]
    public void ShouldBeComposable()
    {
        SqlQueryBuilder innerQuery = $"SELECT * FROM Orders WHERE Id = {123}";
        SqlQueryBuilder outerQuery = $"SELECT * FROM ({innerQuery}) src WHERE IsValid = {true}";

        Assert.Equal(
            "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src WHERE IsValid = @p2", 
            outerQuery.GetQuery());
        Assert.Equal(
            new Dictionary<string, object?> { ["p1"] = 123, ["p2"] = true }, 
            outerQuery.GetParameters());
    }
}