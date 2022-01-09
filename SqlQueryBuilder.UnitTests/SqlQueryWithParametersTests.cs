using SqlQueryBuilder.UnitTests.Utils;
using Xunit;

namespace SqlQueryBuilder.UnitTests;

public sealed class SqlQueryWithParametersTests
{
    [Fact] public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "test";

        query.Should().BeQueryWithParameters("test");
    }
}