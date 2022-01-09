using FluentAssertions;
using FluentAssertions.Primitives;

namespace SqlQueryWithParameters.UnitTests.Utils;

internal sealed class SqlQueryWithParametersAssertions
    : ReferenceTypeAssertions<SqlQueryBuilder, SqlQueryWithParametersAssertions>
{
    public SqlQueryWithParametersAssertions(SqlQueryBuilder instance) 
        : base(instance)
    {
    }

    public AndConstraint<SqlQueryWithParametersAssertions> BeQueryWithParameters(string query)
    {
        Subject.Query.Should().Be(query);
        return new AndConstraint<SqlQueryWithParametersAssertions>(this);
    }

    protected override string Identifier => nameof(SqlQueryBuilder);
}