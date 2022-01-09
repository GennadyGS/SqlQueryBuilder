using SqlQueryBuilder.UnitTests.Utils;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryBuilderTests
{
    [Fact] 
    public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "test";

        query.Should().HaveQuery("test").And.NotHaveParameters();
    }
}