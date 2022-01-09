using SqlQueryWithParameters.UnitTests.Utils;
using Xunit;

namespace SqlQueryWithParameters.UnitTests;

public sealed class SqlQueryWithParametersTests
{
    [Fact] public void ShouldBeImplicitlyCastFromString()
    {
        SqlQueryBuilder query = "test";

        query.Should().BeQueryWithParameters("test");
    }
}