using System.Collections.Generic;
using SqlQueryBuilder.UnitTests.Utils;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact] 
    public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "SELECT * FROM Orders WHERE Id = 123";

        query.Should()
            .HaveQuery("SELECT * FROM Orders WHERE Id = 123")
            .And.NotHaveParameters();
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedString()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders WHERE Id = 123";

        query.Should()
            .HaveQuery("SELECT * FROM Orders WHERE Id = 123")
            .And.NotHaveParameters();
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedStringWithParameters()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders WHERE Id = {123}";

        query.Should()
            .HaveQuery("SELECT * FROM Orders WHERE Id = @p1").
            And.HaveParameters(
                new Dictionary<string, object>
                {
                    ["p1"] = 123,
                });
    }

    [Fact]
    public void ShouldBeComposable()
    {
        SqlQueryBuilder innerQuery = $"SELECT * FROM Orders WHERE Id = {123}";
        SqlQueryBuilder outerQuery = $"SELECT * FROM ({innerQuery}) src WHERE IsValid = {true}";

        outerQuery.Should()
            .HaveQuery(
                "SELECT * FROM (SELECT * FROM Orders WHERE Id = @p1) src" + 
                " WHERE IsValid = @p2")
            .And.HaveParameters(
                new Dictionary<string, object>
                {
                    ["p1"] = 123,
                    ["p2"] = true,
                });
    }
}