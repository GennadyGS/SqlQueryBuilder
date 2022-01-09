using System.Collections.Generic;
using SqlQueryBuilder.UnitTests.Utils;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact] 
    public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "SELECT * FROM Orders Id = 123";

        query.Should()
            .HaveQuery("SELECT * FROM Orders Id = 123")
            .And.NotHaveParameters();
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedString()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders Id = 123";

        query.Should()
            .HaveQuery("SELECT * FROM Orders Id = 123")
            .And.NotHaveParameters();
    }

    [Fact]
    public void ShouldBeImplicitlyCastFromInterpolatedStringWithParameters()
    {
        SqlQueryBuilder query = $"SELECT * FROM Orders Id = {123}";

        query.Should()
            .HaveQuery("SELECT * FROM Orders Id = @p1").
            And.HaveParameters(
                new Dictionary<string, object>
                {
                    ["p1"] = 123,
                });
    }
}